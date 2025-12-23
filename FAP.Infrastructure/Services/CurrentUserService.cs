using FAP.Common.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Linq; // Ensure this is present for LINQ methods
using System;

namespace FAP.API.Backend.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPermissionCacheService _permissionCacheService;

        public CurrentUserService(
            IHttpContextAccessor httpContextAccessor,
            IPermissionCacheService permissionCacheService)
        {
            _httpContextAccessor = httpContextAccessor;
            _permissionCacheService = permissionCacheService;
        }

        public string? UserId =>
            _httpContextAccessor.HttpContext?.User?.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        public string? UserName =>
            _httpContextAccessor.HttpContext?.User?.Identity?.Name;

        public string? Email =>
            _httpContextAccessor.HttpContext?.User?.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        public string? CampusCode =>
            _httpContextAccessor.HttpContext?.User?.Claims
                .FirstOrDefault(c => c.Type == "campus")?.Value;

        public bool IsAuthenticated =>
            _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        /// <summary>
        /// Kiểm tra user có quyền thực thi permission không
        /// Admin role có tất cả quyền
        /// Permissions được lưu trong Redis, không còn trong token
        /// </summary>
        public async Task<bool> HasPermissionAsync(string permission)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true)
            {
                return false;
            }

            var userId = UserId;
            if (string.IsNullOrEmpty(userId))
            {
                return false;
            }

            // Admin có tất cả quyền (kept commented as before)
            // var roles = user.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);

            // Kiểm tra permission từ Redis (async)
            try
            {
                return await _permissionCacheService.HasPermissionAsync(userId, permission);
            }
            catch
            {
                return false;
            }
        }
    }
}


