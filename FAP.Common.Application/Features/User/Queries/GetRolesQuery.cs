using FAP.Common.Application.Attributes;
using FAP.Share.Dtos;
using FAP.Common.Application.Interfaces;
using MediatR;

using Microsoft.EntityFrameworkCore;

namespace FAP.Common.Application.Features.User.Queries
{
    [FapPermission("Role.View")]
    public class GetRolesQuery : IRequest<List<RoleDto>>
    {
        public class Handler : IRequestHandler<GetRolesQuery, List<RoleDto>>
        {
            private readonly IMasterRepository _master;

            public Handler(IMasterRepository master)
            {
                _master = master;
            }

            public async Task<List<RoleDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
            {
                var roles = await _master.GetRolesAsync(cancellationToken);
                var result = new List<RoleDto>();

                foreach (var role in roles)
                {
                    var userCount = await _master.GetUserCountInRoleAsync(role.Name ?? string.Empty, cancellationToken);
                    result.Add(new RoleDto
                    {
                        Id = role.Id,
                        Name = role.Name ?? string.Empty,
                        NormalizedName = role.NormalizedName,
                        UserCount = userCount
                    });
                }

                return result;
            }
        }
    }
}

