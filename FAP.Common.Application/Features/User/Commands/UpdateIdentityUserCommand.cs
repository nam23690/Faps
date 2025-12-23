using FAP.Common.Application.Attributes;
using FAP.Common.Application.Interfaces;
using FAP.Share.Dtos;
using MediatR;

namespace FAP.Common.Application.Features.User.Commands
{
    [FapPermission("User.Update")]
    public class UpdateIdentityUserCommand : IRequest<IdentityUserDto>
    {
        public string Id { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? CampusId { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public bool? EmailConfirmed { get; set; }
        public bool? IsLocked { get; set; }

        public class Handler : IRequestHandler<UpdateIdentityUserCommand, IdentityUserDto>
        {
            private readonly IMasterRepository _master;

            public Handler(IMasterRepository master)
            {
                _master = master;
            }

            public async Task<IdentityUserDto> Handle(UpdateIdentityUserCommand request, CancellationToken cancellationToken)
            {
                var user = await _master.GetUserByIdAsync(request.Id, cancellationToken);
                if (user == null)
                {
                    throw new KeyNotFoundException($"Không tìm thấy user với ID: {request.Id}");
                }

                if (!string.IsNullOrEmpty(request.Email))
                    user.Email = request.Email;

                if (!string.IsNullOrEmpty(request.FullName))
                    user.FullName = request.FullName;

                if (!string.IsNullOrEmpty(request.CampusId))
                    user.CampusId = request.CampusId;

                if (request.ProfilePictureUrl != null)
                    user.ProfilePictureUrl = request.ProfilePictureUrl;

                if (request.EmailConfirmed.HasValue)
                    user.EmailConfirmed = request.EmailConfirmed.Value;

                if (request.IsLocked.HasValue)
                    user.IsLocked = request.IsLocked.Value;

                var result = await _master.UpdateUserAsync(user, cancellationToken);
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException($"Không thể cập nhật user: {string.Join(", ", result.Errors)}");
                }

                var roles = await _master.GetUserRolesAsync(user.Id, cancellationToken);

                return new IdentityUserDto
                {
                    Id = user.Id,
                    UserName = user.UserName!,
                    Email = user.Email!,
                    FullName = user.FullName,
                    CampusId = user.CampusId,
                    EmailConfirmed = user.EmailConfirmed,
                    IsLocked = !user.IsLocked,
                    ProfilePictureUrl = user.ProfilePictureUrl,
                    CreatedAt = user.CreatedAt,
                    Roles = roles.ToList()
                };
            }
        }
    }
}

