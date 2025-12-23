using FAP.Common.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace FAP.Common.Infrastructure.Services
{
    public class PermissionCacheService : IPermissionCacheService
    {
        private readonly IDistributedCache _cache;
        private const string PermissionKeyPrefix = "user:permissions:";
        private static readonly TimeSpan DefaultExpiration = TimeSpan.FromHours(24); // Mặc định 24 giờ

        public PermissionCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task SetUserPermissionsAsync(string userId, List<string> permissions, TimeSpan? expiration = null)
        {
            var key = GetCacheKey(userId);
            var json = JsonSerializer.Serialize(permissions);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? DefaultExpiration
            };

            await _cache.SetStringAsync(key, json, options);
        }

        public async Task<List<string>> GetUserPermissionsAsync(string userId)
        {
            var key = GetCacheKey(userId);
            var json = await _cache.GetStringAsync(key);

            if (string.IsNullOrEmpty(json))
            {
                return new List<string>();
            }

            try
            {
                var permissions = JsonSerializer.Deserialize<List<string>>(json);
                return permissions ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }

        public async Task<bool> HasPermissionAsync(string userId, string permission)
        {
            var permissions = await GetUserPermissionsAsync(userId);
            return permissions.Contains(permission, StringComparer.OrdinalIgnoreCase);
        }

        public async Task RemoveUserPermissionsAsync(string userId)
        {
            var key = GetCacheKey(userId);
            await _cache.RemoveAsync(key);
        }

        private static string GetCacheKey(string userId)
        {
            return $"{PermissionKeyPrefix}{userId}";
        }
    }
}

