using FAP.Common.Application.Attributes;
using FAP.Share.Dtos;
using FAP.Common.Application.Interfaces;
using MediatR;

namespace FAP.Common.Application.Features.User.Queries
{
    [FapPermission("Permission.View")]
    public class GetPermissionsQuery : IRequest<List<PermissionDto>>
    {
        public string? Module { get; set; }
        public bool? IsActive { get; set; }

        public class Handler : IRequestHandler<GetPermissionsQuery, List<PermissionDto>>
        {
            private readonly IMasterRepository _master;

            public Handler(IMasterRepository master)
            {
                _master = master;
            }

            public async Task<List<PermissionDto>> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
            {
                return await _master.GetPermissionsAsDtoAsync(request.Module, request.IsActive, cancellationToken);
            }
        }
    }
}

