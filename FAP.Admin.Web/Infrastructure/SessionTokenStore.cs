using FAP.Shared.Http.Abstractions;
using Microsoft.AspNetCore.Http;

namespace FAP.Admin.Web.Infrastructure;

/// <summary>
/// Session-based implementation of ITokenStore.
/// Stores access tokens and refresh tokens in ASP.NET Core session.
/// 
/// Architecture Decision:
/// - Uses ISession for token persistence (stateless server requirement)
/// - Session keys are constants to avoid magic strings
/// - Thread-safe operations using async/await
/// </summary>
public class SessionTokenStore : ITokenStore
{
    private const string AccessTokenKey = "AccessToken";
    private const string RefreshTokenKey = "RefreshToken";

    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="SessionTokenStore"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor for accessing session.</param>
    public SessionTokenStore(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    private ISession Session => _httpContextAccessor.HttpContext?.Session 
        ?? throw new InvalidOperationException("HttpContext is not available");

    /// <inheritdoc/>
    public Task<string?> GetAccessTokenAsync()
    {
        var token = Session.GetString(AccessTokenKey);
        return Task.FromResult<string?>(token);
    }

    /// <inheritdoc/>
    public Task<string?> GetRefreshTokenAsync()
    {
        var token = Session.GetString(RefreshTokenKey);
        return Task.FromResult<string?>(token);
    }

    /// <inheritdoc/>
    public Task SaveTokensAsync(string accessToken, string refreshToken)
    {
        Session.SetString(AccessTokenKey, accessToken);
        Session.SetString(RefreshTokenKey, refreshToken);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task ClearTokensAsync()
    {
        Session.Remove(AccessTokenKey);
        Session.Remove(RefreshTokenKey);
        return Task.CompletedTask;
    }
}