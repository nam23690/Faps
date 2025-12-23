using FAP.Common.Application.Attributes;
using FAP.Share.Dtos;
using FAP.Common.Application.Interfaces;
using MediatR;


namespace FAP.Common.Application.Features.User.Queries
{
    [FapPermission("User.View")]
    public class GetIdentityUserByIdQuery : IRequest<IdentityUserDto?>
    {
        public string Id { get; set; } = string.Empty;

        public class Handler : IRequestHandler<GetIdentityUserByIdQuery, IdentityUserDto?>
        {
            private readonly IMasterRepository _master;

            public Handler(IMasterRepository master)
            {
                _master = master;
            }

            public async Task<IdentityUserDto?> Handle(GetIdentityUserByIdQuery request, CancellationToken cancellationToken)
            {
                var user = await _master.GetUserByIdAsync(request.Id, cancellationToken);
                if (user == null)
                {
                    return null;
                }

                var roles = await _master.GetUserRolesAsync(user.Id, cancellationToken);
                var claims = await _master.GetUserClaimsAsync(user.Id, cancellationToken);
                var permissions = claims.Where(c => c.Type == "Permission").Select(c => c.Value).ToList();

                return new IdentityUserDto
                {
                    Id = user.Id,
                    UserName = user.UserName!,
                    Email = user.Email!,
                    FullName = user.FullName,
                    CampusId = user.CampusId,
                    EmailConfirmed = user.EmailConfirmed,
                    IsLocked = user.IsLocked,
                    ProfilePictureUrl = user.ProfilePictureUrl,
                    CreatedAt = user.CreatedAt,
                    Roles = roles.ToList(),
                    Permissions = permissions
                };
            }
        }
    }
}

