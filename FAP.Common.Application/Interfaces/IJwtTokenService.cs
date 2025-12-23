using FAP.Share.Dtos;

namespace FAP.Common.Application.Interfaces
{
    public interface IJwtTokenService
    {
        Task<TokenResult> GenerateTokenAsync(string userId);
        string GenerateToken(string userId, string username, string role);
        string GenerateToken(string userId, string username, string role, string email, string campusCode);
    }
}

