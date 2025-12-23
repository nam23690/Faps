namespace FAP.Common.Application.Interfaces
{
    /// <summary>
    /// Service để quản lý refresh tokens
    /// </summary>
    public interface IRefreshTokenService
    {
        /// <summary>
        /// Tạo refresh token mới cho user
        /// </summary>
        Task<FAP.Share.Dtos.RefreshTokenDto> CreateRefreshTokenAsync(
            string userId, 
            string? deviceInfo = null, 
            string? ipAddress = null,
            int expirationDays = 30);

        /// <summary>
        /// Lấy refresh token theo token string
        /// </summary>
        Task<FAP.Share.Dtos.RefreshTokenDto?> GetRefreshTokenAsync(string token);

        /// <summary>
        /// Revoke refresh token (đánh dấu là đã bị hủy)
        /// </summary>
        Task RevokeRefreshTokenAsync(string token);

        /// <summary>
        /// Revoke tất cả refresh tokens của user (dùng khi logout hoặc đổi password)
        /// </summary>
        Task RevokeAllUserTokensAsync(string userId);

        /// <summary>
        /// Xóa các refresh token đã hết hạn (cleanup)
        /// </summary>
        Task CleanupExpiredTokensAsync();
    }
}

