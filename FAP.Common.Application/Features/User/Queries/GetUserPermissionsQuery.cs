using FAP.Common.Application.Attributes;
using FAP.Share.Dtos;
using FAP.Common.Application.Interfaces;
using MediatR;


namespace FAP.Common.Application.Features.User.Queries
{
    [FapPermission("Permission.View")]
    public class GetUserPermissionsQuery : IRequest<List<string>>
    {
        public string UserId { get; set; } = string.Empty;

        public class Handler : IRequestHandler<GetUserPermissionsQuery, List<string>>
        {
            private readonly IMasterRepository _master;

            public Handler(IMasterRepository master)
            {
                _master = master;
            }

            public async Task<List<string>> Handle(GetUserPermissionsQuery request, CancellationToken cancellationToken)
            {
                var user = await _master.GetUserByIdAsync(request.UserId, cancellationToken);
                if (user == null)
                {
                    throw new KeyNotFoundException($"Không tìm thấy user với ID: {request.UserId}");
                }

                var claims = await _master.GetUserClaimsAsync(request.UserId, cancellationToken);
                return claims.Where(c => c.Type == "Permission").Select(c => c.Value).ToList();
            }
        }
    }
}

