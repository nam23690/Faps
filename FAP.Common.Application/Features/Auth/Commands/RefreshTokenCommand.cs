using FAP.Common.Application.Interfaces;
using MediatR;

using System.Security.Claims;

namespace FAP.Common.Application.Features.Auth.Commands
{
    /// <summary>
    /// Command để refresh access token bằng refresh token
    /// </summary>
    public class RefreshTokenCommand : IRequest<RefreshTokenResponse>
    {
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
    {
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IPermissionCacheService _permissionCacheService;
        private readonly IMasterRepository _master;

        public RefreshTokenCommandHandler(
            IRefreshTokenService refreshTokenService,
            IJwtTokenService jwtTokenService,
            IPermissionCacheService permissionCacheService,
            IMasterRepository master)
        {
            _refreshTokenService = refreshTokenService;
            _jwtTokenService = jwtTokenService;
            _permissionCacheService = permissionCacheService;
            _master = master;
        }

        public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            // Lấy refresh token từ database
            var refreshToken = await _refreshTokenService.GetRefreshTokenAsync(request.RefreshToken);

            if (refreshToken == null)
            {
                throw new UnauthorizedAccessException("Refresh token không hợp lệ");
            }

            if (!refreshToken.IsValid)
            {
                throw new UnauthorizedAccessException("Refresh token đã hết hạn hoặc đã bị hủy");
            }

            // Lấy user
            var user = await _master.GetUserByIdAsync(refreshToken.UserId, cancellationToken);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User không tồn tại");
            }

            // Kiểm tra user có bị khóa không
            if (user.IsLocked)
            {
                throw new UnauthorizedAccessException("Tài khoản đã bị khóa");
            }

            // Lấy roles và permissions
            var roles = await _master.GetUserRolesAsync(user.Id, cancellationToken);
            var rolesString = roles.Any() ? string.Join(",", roles) : "user";

            // Lấy permissions từ Redis
            var permissions = await _permissionCacheService.GetUserPermissionsAsync(user.Id);
            
            // Nếu không có trong Redis, lấy từ claims (fallback)
            if (!permissions.Any())
            {
                var allClaims = new List<System.Security.Claims.Claim>();
                var userClaims = await _master.GetUserClaimsAsync(user.Id, cancellationToken);
                allClaims.AddRange(userClaims);

                foreach (var roleName in roles)
                {
                    var role = await _master.FindRoleByNameAsync(roleName, cancellationToken);
                    if (role == null) continue;
                    var roleClaims = await _master.GetRoleClaimsAsync(role.Id, cancellationToken);
                    allClaims.AddRange(roleClaims);
                }

                permissions = allClaims
                    .Where(c => c.Type == "Permission")
                    .Select(c => c.Value)
                    .Distinct()
                    .ToList();

                // Lưu lại vào Redis
                if (permissions.Any())
                {
                    await _permissionCacheService.SetUserPermissionsAsync(user.Id, permissions);
                }
            }

            var campusCode = user.CampusId ?? string.Empty;

            // Tạo access token mới
            var newAccessToken = _jwtTokenService.GenerateToken(
                user.Id,
                user.UserName ?? user.Email ?? string.Empty,
                rolesString,
                user.Email ?? string.Empty,
                campusCode
            );

            // Theo chuẩn hệ thống lớn: có thể tạo refresh token mới (rotation) hoặc giữ nguyên
            // Ở đây ta sẽ giữ nguyên refresh token để đơn giản
            // Nếu muốn rotation, uncomment phần dưới:
            /*
            // Revoke token cũ
            await _refreshTokenService.RevokeRefreshTokenAsync(request.RefreshToken);
            
            // Tạo refresh token mới
            var newRefreshToken = await _refreshTokenService.CreateRefreshTokenAsync(
                user.Id,
                refreshToken.DeviceInfo,
                refreshToken.IpAddress,
                expirationDays: 30
            );
            */

            return new RefreshTokenResponse
            {
                Token = newAccessToken,
                RefreshToken = refreshToken.Token // Giữ nguyên refresh token
            };
        }
    }

    public class RefreshTokenResponse
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}

