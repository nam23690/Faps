using FAP.Common.Application.Interfaces;
using FAP.Share.Dtos;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FAP.Common.Application.Features.Auth.Commands
{
    public class LoginFeIdCommand : IRequest<LoginResponse>
    {
        public string AccessToken { get; set; } = string.Empty;
        public string? CampusCode { get; set; }

        public class Handler : IRequestHandler<LoginFeIdCommand, LoginResponse>
        {
            private readonly IMasterRepository _master;
            private readonly IJwtTokenService _tokenService;
            private readonly IPermissionCacheService _permissionCacheService;
            private readonly IRefreshTokenService _refreshTokenService;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(
                IMasterRepository master,
                IJwtTokenService tokenService,
                IPermissionCacheService permissionCacheService,
                IRefreshTokenService refreshTokenService,
                IHttpContextAccessor httpContextAccessor)
            {
                _master = master;
                _tokenService = tokenService;
                _permissionCacheService = permissionCacheService;
                _refreshTokenService = refreshTokenService;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<LoginResponse> Handle(LoginFeIdCommand request, CancellationToken cancellationToken)
            {
                // Giải mã access token từ FE ID để lấy thông tin user
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(request.AccessToken);

                if (token.ValidTo < DateTime.UtcNow)
                {
                    throw new UnauthorizedAccessException("AccessToken hết hạn");
                }

                var feid = token.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
                var userName = token.Claims.FirstOrDefault(c => c.Type == "username")?.Value;
                var email = token.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
                var type = token.Claims.FirstOrDefault(c => c.Type == "userType")?.Value;

                if (string.IsNullOrEmpty(feid) || string.IsNullOrEmpty(email) )
                {
                    throw new UnauthorizedAccessException("AccessToken không hợp lệ");
                }
                // Tìm user theo username hoặc email
                var user =  await _master.FindByEmailAsync(email, cancellationToken);

                if (user == null)
                {
                    // Nếu user không tồn tại trả về lỗi
                    throw new UnauthorizedAccessException($"Người dùng không tồn tại email {email}");
                }

                // Kiểm tra user có bị khóa không
                if (user.IsLocked)
                {
                    throw new UnauthorizedAccessException("Tài khoản đã bị khóa");
                }

                // Lấy roles của user
                var roles = await _master.GetUserRolesAsync(user.Id, cancellationToken);
                var rolesString = roles.Any() ? string.Join(",", roles) : "user";

                // 5️⃣ Lấy ĐỦ claims (UserClaims + RoleClaims)
                var allClaims = new List<Claim>();

                // User claims (nếu có dùng)
                var userClaims = await _master.GetUserClaimsAsync(user.Id, cancellationToken);
                allClaims.AddRange(userClaims);

                // Role claims (CHÍNH – chứa Permission)
                foreach (var roleName in roles)
                {
                    var role = await _master.FindRoleByNameAsync(roleName, cancellationToken);
                    if (role == null) continue;

                    var roleClaims = await _master.GetRoleClaimsAsync(role.Id, cancellationToken);
                    allClaims.AddRange(roleClaims);
                }

                // 6️⃣ Loại trùng claim
                allClaims = allClaims
                    .GroupBy(c => new { c.Type, c.Value })
                    .Select(g => g.First())
                    .ToList();

                // 7️⃣ Lấy permissions
                var permissions = allClaims
                    .Where(c => c.Type == "Permission")
                    .Select(c => c.Value)
                    .Distinct()
                    .ToList();

                // Lấy campus code từ user hoặc request
                var campusCode = request.CampusCode ?? user.CampusId ?? string.Empty;

                // Lưu permissions vào Redis thay vì đưa vào token
                await _permissionCacheService.SetUserPermissionsAsync(user.Id, permissions);

                // Tạo access token (không còn chứa permissions, chỉ có thông tin cơ bản)
                var accessToken = _tokenService.GenerateToken(
                    user.Id,
                    user.UserName ?? user.Email ?? string.Empty,
                    rolesString,
                    user.Email ?? string.Empty,
                    campusCode
                );

                // Tạo refresh token
                var httpContext = _httpContextAccessor.HttpContext;
                var deviceInfo = httpContext?.Request.Headers["User-Agent"].ToString();
                var ipAddress = httpContext?.Connection.RemoteIpAddress?.ToString();
                
                var refreshToken = await _refreshTokenService.CreateRefreshTokenAsync(
                    user.Id,
                    deviceInfo,
                    ipAddress,
                    expirationDays: 30 // Refresh token sống 30 ngày
                );

                return new LoginResponse
                {
                    Token = accessToken,
                    RefreshToken = refreshToken.Token,
                    UserId = user.Id,
                    Username = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    FullName = user.FullName,
                    Roles = roles.ToList(),
                    Permissions = permissions,
                    CampusCode = campusCode
                };
            }
        }
    }
}

