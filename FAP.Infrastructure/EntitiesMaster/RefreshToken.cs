namespace FAP.Common.Infrastructure.EntitiesMaster
{
    /// <summary>
    /// Entity để lưu refresh token trong database
    /// Theo chuẩn hệ thống lớn: refresh token được lưu trong DB để có thể revoke
    /// </summary>
    public class RefreshToken
    {
        public int Id { get; set; }
        
        /// <summary>
        /// Token string (random, unique)
        /// </summary>
        public string Token { get; set; } = string.Empty;
        
        /// <summary>
        /// User ID (FK to ApplicationUser)
        /// </summary>
        public string UserId { get; set; } = string.Empty;
        
        /// <summary>
        /// Thời điểm hết hạn
        /// </summary>
        public DateTime ExpiresAt { get; set; }
        
        /// <summary>
        /// Đã bị revoke chưa
        /// </summary>
        public bool IsRevoked { get; set; }
        
        /// <summary>
        /// Thời điểm bị revoke
        /// </summary>
        public DateTime? RevokedAt { get; set; }
        
        /// <summary>
        /// Thời điểm tạo
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Thông tin device (optional)
        /// </summary>
        public string? DeviceInfo { get; set; }
        
        /// <summary>
        /// IP address khi tạo token (optional)
        /// </summary>
        public string? IpAddress { get; set; }
        
        /// <summary>
        /// Navigation property
        /// </summary>
        public ApplicationUser? User { get; set; }
        
        /// <summary>
        /// Kiểm tra token còn hợp lệ không
        /// </summary>
        public bool IsValid => !IsRevoked && ExpiresAt > DateTime.UtcNow;
    }
}

