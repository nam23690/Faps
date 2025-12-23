using FAP.Common.Application.Interfaces;
using FAP.Common.Infrastructure.EntitiesMaster;
using FAP.Common.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace FAP.Common.Infrastructure.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly MasterDbContext _dbContext;

        public RefreshTokenService(MasterDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<FAP.Share.Dtos.RefreshTokenDto> CreateRefreshTokenAsync(
            string userId, 
            string? deviceInfo = null, 
            string? ipAddress = null,
            int expirationDays = 30)
        {
            // Tạo token ngẫu nhiên, an toàn
            var token = GenerateSecureToken();

            var refreshToken = new RefreshToken
            {
                Token = token,
                UserId = userId,
                ExpiresAt = DateTime.UtcNow.AddDays(expirationDays),
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false,
                DeviceInfo = deviceInfo,
                IpAddress = ipAddress
            };

            _dbContext.RefreshTokens.Add(refreshToken);
            await _dbContext.SaveChangesAsync();

            return new FAP.Share.Dtos.RefreshTokenDto
            {
                Id = refreshToken.Id,
                Token = refreshToken.Token,
                UserId = refreshToken.UserId,
                ExpiresAt = refreshToken.ExpiresAt,
                IsRevoked = refreshToken.IsRevoked,
                RevokedAt = refreshToken.RevokedAt,
                CreatedAt = refreshToken.CreatedAt,
                DeviceInfo = refreshToken.DeviceInfo,
                IpAddress = refreshToken.IpAddress
            ,
                IsValid = !refreshToken.IsRevoked && refreshToken.ExpiresAt > DateTime.UtcNow
            };
        }

        public async Task<FAP.Share.Dtos.RefreshTokenDto?> GetRefreshTokenAsync(string token)
        {
            var rt = await _dbContext.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Token == token);

            if (rt == null) return null;

            return new FAP.Share.Dtos.RefreshTokenDto
            {
                Id = rt.Id,
                Token = rt.Token,
                UserId = rt.UserId,
                ExpiresAt = rt.ExpiresAt,
                IsRevoked = rt.IsRevoked,
                RevokedAt = rt.RevokedAt,
                CreatedAt = rt.CreatedAt,
                DeviceInfo = rt.DeviceInfo,
                IpAddress = rt.IpAddress,
                IsValid = !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow
            };
        }

        public async Task RevokeRefreshTokenAsync(string token)
        {
            var rt = await _dbContext.RefreshTokens.FirstOrDefaultAsync(r => r.Token == token);
            if (rt != null && !rt.IsRevoked)
            {
                rt.IsRevoked = true;
                rt.RevokedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task RevokeAllUserTokensAsync(string userId)
        {
            var tokens = await _dbContext.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
            }

            if (tokens.Any())
            {
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task CleanupExpiredTokensAsync()
        {
            // Xóa các token đã hết hạn và đã bị revoke (giữ lại để audit)
            // Hoặc có thể xóa tất cả token hết hạn sau một thời gian nhất định
            var expiredTokens = await _dbContext.RefreshTokens
                .Where(rt => rt.ExpiresAt < DateTime.UtcNow.AddDays(-7) && rt.IsRevoked)
                .ToListAsync();

            if (expiredTokens.Any())
            {
                _dbContext.RefreshTokens.RemoveRange(expiredTokens);
                await _dbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Tạo token ngẫu nhiên, an toàn (64 bytes = 128 ký tự hex)
        /// </summary>
        private static string GenerateSecureToken()
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[64];
            rng.GetBytes(bytes);
            return Convert.ToHexString(bytes);
        }
    }
}

