using FAP.Common.Application.Attributes;
using FAP.Common.Application.Interfaces;
using MediatR;


namespace FAP.Common.Application.Features.User.Commands
{
    [FapPermission("User.Delete")]
    public class DeleteIdentityUserCommand : IRequest<Unit>
    {
        public string Id { get; set; } = string.Empty;

        public class Handler : IRequestHandler<DeleteIdentityUserCommand, Unit>
        {
            private readonly IMasterRepository _master;
            private readonly IPermissionCacheService _permissionCacheService;

            public Handler(IMasterRepository master, IPermissionCacheService permissionCacheService)
            {
                _master = master;
                _permissionCacheService = permissionCacheService;
            }

            public async Task<Unit> Handle(DeleteIdentityUserCommand request, CancellationToken cancellationToken)
            {
                var user = await _master.GetUserByIdAsync(request.Id, cancellationToken);
                if (user == null)
                {
                    throw new KeyNotFoundException($"Không tìm thấy user với ID: {request.Id}");
                }

                var result = await _master.DeleteUserAsync(user, cancellationToken);
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException($"Không thể xóa user: {string.Join(", ", result.Errors)}");
                }

                try
                {
                    await _permissionCacheService.RemoveUserPermissionsAsync(request.Id);
                }
                catch
                {
                }

                return Unit.Value;
            }
        }
    }
}

