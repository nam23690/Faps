using FAP.Common.Application.Attributes;

using FAP.Share.Dtos;
using FAP.Common.Application.Interfaces;
using MediatR;

using Microsoft.EntityFrameworkCore;

namespace FAP.Common.Application.Features.User.Queries
{
    [FapPermission("User.View")]
    public class GetIdentityUsersPagingQuery : IRequest<PagedResult<IdentityUserDto>>
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Keyword { get; set; }

        public class Handler : IRequestHandler<GetIdentityUsersPagingQuery, PagedResult<IdentityUserDto>>
        {
            private readonly IMasterRepository _master;

            public Handler(IMasterRepository master)
            {
                _master = master;
            }

            public async Task<PagedResult<IdentityUserDto>> Handle(GetIdentityUsersPagingQuery request, CancellationToken cancellationToken)
            {
                var pageIndex = request.PageIndex < 1 ? 1 : request.PageIndex;
                var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;
                var (users, total) = await _master.GetUsersPagingAsync(pageIndex, pageSize, request.Keyword, cancellationToken);

                var items = new List<IdentityUserDto>();

                foreach (var user in users)
                {
                        var roles = await _master.GetUserRolesAsync(user.Id, cancellationToken);
                        var claims = await _master.GetUserClaimsAsync(user.Id, cancellationToken);
                    var permissions = claims
                        .Where(c => c.Type == "Permission")
                        .Select(c => c.Value)
                        .ToList();

                    items.Add(new IdentityUserDto
                    {
                        Id = user.Id,
                        UserName = user.UserName ?? string.Empty,
                        Email = user.Email ?? string.Empty,
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

                return new PagedResult<IdentityUserDto>
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalRecords = total,
                    Items = items
                };
            }
        }
    }
}

