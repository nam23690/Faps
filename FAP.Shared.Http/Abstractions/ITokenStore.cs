namespace FAP.Shared.Http.Abstractions;

/// <summary>
/// Abstract storage interface for JWT access tokens and refresh tokens.
/// This abstraction allows different implementations (Session, Memory, etc.) 
/// without coupling the HTTP client infrastructure to any specific storage mechanism.
/// </summary>
public interface ITokenStore
{
    /// <summary>
    /// Gets the current access token.
    /// </summary>
    /// <returns>The access token if available; otherwise, null.</returns>
    Task<string?> GetAccessTokenAsync();

    /// <summary>
    /// Gets the current refresh token.
    /// </summary>
    /// <returns>The refresh token if available; otherwise, null.</returns>
    Task<string?> GetRefreshTokenAsync();

    /// <summary>
    /// Saves the access token and refresh token.
    /// </summary>
    /// <param name="accessToken">The access token to save.</param>
    /// <param name="refreshToken">The refresh token to save.</param>
    Task SaveTokensAsync(string accessToken, string refreshToken);

    /// <summary>
    /// Clears both access token and refresh token.
    /// </summary>
    Task ClearTokensAsync();
}

