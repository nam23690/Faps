namespace FAP.Shared.Http.Models;

/// <summary>
/// Result of a token refresh operation.
/// Maps to API response structure.
/// </summary>
public class RefreshTokenResult
{
    /// <summary>
    /// Gets or sets a value indicating whether the refresh operation was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the new access token (mapped from API's "Token" property).
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the new refresh token.
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;
}

