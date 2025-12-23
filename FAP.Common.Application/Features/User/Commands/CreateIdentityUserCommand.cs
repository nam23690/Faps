using FAP.Common.Application.Attributes;
using FAP.Share.Dtos;
using FAP.Common.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FAP.Common.Application.Features.User.Commands
{
    [FapPermission("User.Create")]
    public class CreateIdentityUserCommand : IRequest<IdentityUserDto>
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string CampusId { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public List<string> RoleNames { get; set; } = new();
        public string? CampusCode { get; set; }

        public class Handler : IRequestHandler<CreateIdentityUserCommand, IdentityUserDto>
        {
            private readonly IMasterRepository _master;
            private readonly IPermissionCacheService _permissionCacheService;

            public Handler(IMasterRepository master, IPermissionCacheService permissionCacheService)
            {
                _master = master;
                _permissionCacheService = permissionCacheService;
            }

            public async Task<IdentityUserDto> Handle(CreateIdentityUserCommand request, CancellationToken cancellationToken)
            {
                var userDto = new IdentityUserDto
                {
                    UserName = request.UserName,
                    Email = request.Email,
                    FullName = request.FullName,
                    CampusId = request.CampusId,
                    ProfilePictureUrl = request.ProfilePictureUrl ?? string.Empty,
                    CreatedAt = DateTime.UtcNow,
                    IsLocked = false,
                    EmailConfirmed = true
                };

                var result = await _master.CreateUserAsync(userDto, request.Password, cancellationToken);
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException($"Không thể tạo user: {string.Join(", ", result.Errors)}");
                }

                // Assign roles
                if (request.RoleNames.Any())
                {
                    // Add by role names via UserManager inside repository
                    await _master.AddUserToRolesByNamesAsync(userDto, request.RoleNames, cancellationToken);
                }

                // Load user with roles
                var createdUser = await _master.GetUserByIdAsync(userDto.Id, cancellationToken);
                var roles = createdUser != null ? await _master.GetUserRolesAsync(createdUser.Id, cancellationToken) : new List<string>();

                try
                {
                    var allClaims = new List<System.Security.Claims.Claim>();
                    var userClaims = await _master.GetUserClaimsAsync(createdUser!.Id, cancellationToken);
                    allClaims.AddRange(userClaims);

                    foreach (var roleName in roles)
                    {
                        var role = await _master.FindRoleByNameAsync(roleName, cancellationToken);
                        if (role == null) continue;

                        var roleClaims = await _master.GetRoleClaimsAsync(role.Id, cancellationToken);
                        allClaims.AddRange(roleClaims);
                    }

                    allClaims = allClaims
                        .GroupBy(c => new { c.Type, c.Value })
                        .Select(g => g.First())
                        .ToList();

                    var permissions = allClaims
                        .Where(c => c.Type == "Permission")
                        .Select(c => c.Value)
                        .Distinct()
                        .ToList();

                    if (permissions.Any())
                    {
                        await _permissionCacheService.SetUserPermissionsAsync(userDto.Id, permissions);
                    }
                    else
                    {
                        await _permissionCacheService.RemoveUserPermissionsAsync(userDto.Id);
                    }
                }
                catch
                {
                    // swallow cache errors
                }

                return createdUser ?? new IdentityUserDto
                {
                    Id = userDto.Id,
                    UserName = userDto.UserName,
                    Email = userDto.Email,
                    FullName = userDto.FullName,
                    CampusId = userDto.CampusId,
                    EmailConfirmed = userDto.EmailConfirmed,
                    IsLocked = userDto.IsLocked,
                    ProfilePictureUrl = userDto.ProfilePictureUrl,
                    CreatedAt = userDto.CreatedAt,
                    Roles = roles.ToList()
                };
            }
        }
    }
}

