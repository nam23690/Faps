namespace FAP.Common.Application.Interfaces
{
    /// <summary>
    /// Service để quản lý permissions trong Redis cache
    /// </summary>
    public interface IPermissionCacheService
    {
        /// <summary>
        /// Lưu permissions của user vào Redis
        /// </summary>
        Task SetUserPermissionsAsync(string userId, List<string> permissions, TimeSpan? expiration = null);

        /// <summary>
        /// Lấy permissions của user từ Redis
        /// </summary>
        Task<List<string>> GetUserPermissionsAsync(string userId);

        /// <summary>
        /// Kiểm tra user có permission cụ thể không
        /// </summary>
        Task<bool> HasPermissionAsync(string userId, string permission);

        /// <summary>
        /// Xóa permissions của user khỏi Redis
        /// </summary>
        Task RemoveUserPermissionsAsync(string userId);
    }
}

