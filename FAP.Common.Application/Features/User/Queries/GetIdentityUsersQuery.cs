using FAP.Common.Application.Attributes;
using FAP.Share.Dtos;
using FAP.Common.Application.Interfaces;
using MediatR;

using Microsoft.EntityFrameworkCore;

namespace FAP.Common.Application.Features.User.Queries
{
    [FapPermission("User.View")]
    public class GetIdentityUsersQuery : IRequest<List<IdentityUserDto>>
    {
        public class Handler : IRequestHandler<GetIdentityUsersQuery, List<IdentityUserDto>>
        {
            private readonly IMasterRepository _master;

            public Handler(IMasterRepository master)
            {
                _master = master;
            }

            public async Task<List<IdentityUserDto>> Handle(GetIdentityUsersQuery request, CancellationToken cancellationToken)
            {
                var users = await _master.GetAllUsersAsync(cancellationToken);
                var result = new List<IdentityUserDto>();

                foreach (var user in users)
                {
                    var roles = await _master.GetUserRolesAsync(user.Id, cancellationToken);
                    var claims = await _master.GetUserClaimsAsync(user.Id, cancellationToken);
                    var permissions = claims.Where(c => c.Type == "Permission").Select(c => c.Value).ToList();

                    result.Add(new IdentityUserDto
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
                    });
                }

                return result;
            }
        }
    }
}

