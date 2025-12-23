using System.Net.Http.Headers;
using System.Net;
using FAP.Shared.Http.Abstractions;
using FAP.Shared.Http.Models;

namespace FAP.Shared.Http.Handlers;

/// <summary>
/// HTTP message handler that automatically attaches JWT Bearer tokens to requests
/// and handles token refresh on 401 Unauthorized responses.
/// 
/// Architecture Decision:
/// - Does NOT decode JWT tokens (separation of concerns)
/// - Relies on HTTP 401 status code to trigger refresh
/// - Uses ITokenStore abstraction for token persistence
/// - Thread-safe token refresh using SemaphoreSlim
/// </summary>
public class JwtDelegatingHandler : DelegatingHandler
{
    private readonly ITokenStore _tokenStore;
    private readonly IAuthApi _authApi;
    private readonly SemaphoreSlim _refreshSemaphore = new(1, 1);

    /// <summary>
    /// Initializes a new instance of the <see cref="JwtDelegatingHandler"/> class.
    /// </summary>
    /// <param name="tokenStore">The token store for accessing and saving tokens.</param>
    /// <param name="authApi">The authentication API client for refreshing tokens.</param>
    public JwtDelegatingHandler(ITokenStore tokenStore, IAuthApi authApi)
    {
        _tokenStore = tokenStore ?? throw new ArgumentNullException(nameof(tokenStore));
        _authApi = authApi ?? throw new ArgumentNullException(nameof(authApi));
    }

    /// <inheritdoc/>
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Attach access token to request
        var accessToken = await _tokenStore.GetAccessTokenAsync();
        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        // Send the request
        var response = await base.SendAsync(request, cancellationToken);

        // Handle 401 Unauthorized - attempt token refresh
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            // Use semaphore to prevent concurrent refresh attempts
            await _refreshSemaphore.WaitAsync(cancellationToken);
            try
            {
                // Double-check: another thread might have already refreshed
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    var refreshToken = await _tokenStore.GetRefreshTokenAsync();
                    if (!string.IsNullOrWhiteSpace(refreshToken))
                    {
                        var refreshResult = await _authApi.RefreshAsync(refreshToken, cancellationToken);

                        if (refreshResult.Success && 
                            !string.IsNullOrWhiteSpace(refreshResult.AccessToken) &&
                            !string.IsNullOrWhiteSpace(refreshResult.RefreshToken))
                        {
                            // Save new tokens
                            await _tokenStore.SaveTokensAsync(
                                refreshResult.AccessToken,
                                refreshResult.RefreshToken);

                            // Retry the original request with new token
                            request.Headers.Authorization = new AuthenticationHeaderValue(
                                "Bearer", 
                                refreshResult.AccessToken);

                            // Dispose the original response before retrying
                            response.Dispose();

                            response = await base.SendAsync(request, cancellationToken);
                        }
                        else
                        {
                            // Refresh failed - clear tokens
                            await _tokenStore.ClearTokensAsync();
                        }
                    }
                    else
                    {
                        // No refresh token available - clear tokens
                        await _tokenStore.ClearTokensAsync();
                    }
                }
            }
            finally
            {
                _refreshSemaphore.Release();
            }
        }

        return response;
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _refreshSemaphore?.Dispose();
        }
        base.Dispose(disposing);
    }
}

