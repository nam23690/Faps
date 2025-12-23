using FAP.Common.Application.Attributes;
using MediatR;

using FAP.Common.Application.Interfaces;

namespace FAP.Common.Application.Features.User.Commands
{
    [FapPermission("Permission.Assign")]
    public class AssignPermissionCommand : IRequest<Unit>
    {
        public string UserId { get; set; } = string.Empty;
        public List<string> PermissionCodes { get; set; } = new();
        public string? CampusCode { get; set; }

        public class Handler : IRequestHandler<AssignPermissionCommand, Unit>
        {
            private readonly IMasterRepository _master;
            private readonly IUnitOfWork _uow;
            private readonly IPermissionCacheService _permissionCacheService;

            public Handler(
                IMasterRepository master,
                IUnitOfWork uow,
                IPermissionCacheService permissionCacheService)
            {
                _master = master;
                _uow = uow;
                _permissionCacheService = permissionCacheService;
            }

            public async Task<Unit> Handle(AssignPermissionCommand request, CancellationToken cancellationToken)
            {
                var user = await _master.GetUserByIdAsync(request.UserId, cancellationToken);
                if (user == null)
                {
                    throw new KeyNotFoundException($"Không tìm thấy user với ID: {request.UserId}");
                }

                // Get permissions from AppPermissions table
                var permissions = await _master.GetActivePermissionsByCodesAsync(request.PermissionCodes, cancellationToken);

                if (!permissions.Any())
                {
                    throw new InvalidOperationException("Không tìm thấy permission nào hợp lệ");
                }

                // Remove existing permission claims
                var existingClaims = await _master.GetUserClaimsAsync(user.Id, cancellationToken);
                var permissionClaims = existingClaims.Where(c => c.Type == "Permission").ToList();
                if (permissionClaims.Any())
                {
                    await _master.RemoveClaimsFromUserAsync(user.Id, permissionClaims, cancellationToken);
                }

                // Add new permission claims
                var claims = permissions.Select(p => new System.Security.Claims.Claim("Permission", p.Code)).ToList();
                if (claims.Any())
                {
                    await _master.AddClaimsToUserAsync(user.Id, claims, cancellationToken);

                    // Update CampusCode in UserClaims if provided
                    if (!string.IsNullOrEmpty(request.CampusCode))
                    {
                        await _master.UpdateUserClaimCampusCodeAsync(request.UserId, "Permission", request.CampusCode, cancellationToken);
                        await _master.SaveChangesAsync(cancellationToken);
                    }

                    // Rebuild permissions for the user from user claims + role claims and update Redis
                    var allClaims = new List<System.Security.Claims.Claim>();
                    var userClaims = await _master.GetUserClaimsAsync(user.Id, cancellationToken);
                    allClaims.AddRange(userClaims);

                    var roles = await _master.GetUserRolesAsync(user.Id, cancellationToken);
                    foreach (var roleName in roles)
                    {
                        var role = await _master.FindRoleByNameAsync(roleName, cancellationToken);
                        if (role == null) continue;
                        var roleClaims = await _master.GetRoleClaimsAsync(role.Id, cancellationToken);
                        allClaims.AddRange(roleClaims);
                    }

                    var permissionsAll = allClaims
                        .Where(c => c.Type == "Permission")
                        .Select(c => c.Value)
                        .Distinct()
                        .ToList();

                    try
                    {
                        if (permissionsAll.Any())
                        {
                            await _permissionCacheService.SetUserPermissionsAsync(request.UserId, permissionsAll);
                        }
                        else
                        {
                            await _permissionCacheService.RemoveUserPermissionsAsync(request.UserId);
                        }
                    }
                    catch (Exception)
                    {
                        // Swallow exceptions from cache update to avoid breaking permission assignment.
                    }
                }

                return Unit.Value;
            }
        }
    }
}

