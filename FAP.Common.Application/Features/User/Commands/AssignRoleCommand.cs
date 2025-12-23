using FAP.Common.Application.Attributes;
using FAP.Share.Dtos;
using FAP.Common.Application.Interfaces;
using MediatR;

using Microsoft.EntityFrameworkCore;

namespace FAP.Common.Application.Features.User.Commands
{
    [FapPermission("Role.Assign")]
    public class AssignRoleCommand : IRequest<Unit>
    {
        public string UserId { get; set; } = string.Empty;
        public List<string> RoleNames { get; set; } = new();
        public string? CampusCode { get; set; }

        public class Handler : IRequestHandler<AssignRoleCommand, Unit>
        {
            private readonly IMasterRepository _master;
            private readonly IUnitOfWork _uow;
            private readonly IPermissionCacheService _permissionCacheService;

            public Handler (
                IMasterRepository master,
                IUnitOfWork uow,
                IPermissionCacheService permissionCacheService)
            {
                _master = master;
                _uow = uow;
                _permissionCacheService = permissionCacheService;
            }

            public async Task<Unit> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
            {
                var user = await _master.GetUserByIdAsync(request.UserId, cancellationToken);
                if (user == null)
                {
                    throw new KeyNotFoundException($"Không tìm thấy user với ID: {request.UserId}");
                }

                // Get current roles
                var currentRoles = await _master.GetUserRolesAsync(user.Id, cancellationToken);

                // Remove all current roles
                if (currentRoles.Any())
                {
                    await _master.RemoveUserFromRolesByNamesAsync(user, currentRoles, cancellationToken);
                }

                // Add new roles
                if (request.RoleNames.Any())
                {
                    var allRoles = await _master.GetRolesAsync(cancellationToken);
                    var validRoles = allRoles.Where(r => r.Name != null && request.RoleNames.Contains(r.Name)).ToList();

                    if (validRoles.Any())
                    {
                        var roleNames = validRoles.Where(r => r.Name != null).Select(r => r.Name!).ToList();
                        await _master.AddUserToRolesByNamesAsync(user, roleNames, cancellationToken);

                        // Update CampusCode in UserRoles if provided
                        if (!string.IsNullOrEmpty(request.CampusCode))
                        {
                            var roleIds = validRoles.Select(r => r.Id).ToList();
                            await _master.UpdateUserRoleCampusCodeAsync(request.UserId, roleIds, request.CampusCode, cancellationToken);
                            await _master.SaveChangesAsync(cancellationToken);
                        }

                        // Rebuild permissions for the user from user claims + role claims and update Redis
                        var allClaims = new List<System.Security.Claims.Claim>();
                        var userClaims = await _master.GetUserClaimsAsync(user.Id, cancellationToken);
                        allClaims.AddRange(userClaims);
                        foreach (var roleName in roleNames)
                        {
                            var role = await _master.FindRoleByNameAsync(roleName, cancellationToken);
                            if (role == null) continue;
                            var roleClaims = await _master.GetRoleClaimsAsync(role.Id, cancellationToken);
                            allClaims.AddRange(roleClaims);
                        }

                        var permissions = allClaims
                            .Where(c => c.Type == "Permission")
                            .Select(c => c.Value)
                            .Distinct()
                            .ToList();

                        try
                        {
                            if (permissions.Any())
                            {
                                await _permissionCacheService.SetUserPermissionsAsync(request.UserId, permissions);
                            }
                            else
                            {
                                await _permissionCacheService.RemoveUserPermissionsAsync(request.UserId);
                            }
                        }
                        catch (Exception)
                        {
                            // Swallow exceptions from cache update to avoid breaking role assignment.
                        }
                    }
                }

                return Unit.Value;
            }
        }
    }
}

