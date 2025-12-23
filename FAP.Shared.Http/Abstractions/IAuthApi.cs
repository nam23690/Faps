using FAP.Shared.Http.Models;

namespace FAP.Shared.Http.Abstractions;

/// <summary>
/// Interface for authentication API operations.
/// Separated from IApiClient to follow Single Responsibility Principle.
/// </summary>
public interface IAuthApi
{
    /// <summary>
    /// Refreshes the access token using a refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token to use.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The refresh token result containing new tokens.</returns>
    Task<RefreshTokenResult> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default);
}

