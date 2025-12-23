using System.Net.Http.Json;
using System.Text.Json;
using FAP.Shared.Http.Abstractions;
using FAP.Shared.Http.Models;

namespace FAP.Shared.Http.Clients;

/// <summary>
/// API client for authentication operations.
/// Implements IAuthApi to provide token refresh functionality.
/// </summary>
public class AuthApiClient : ApiClientBase, IAuthApi
{
    private const string RefreshEndpoint = "/api/auth/refresh-token";

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthApiClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client to use for requests.</param>
    public AuthApiClient(HttpClient httpClient)
        : base(httpClient)
    {
    }

    /// <inheritdoc/>
    public async Task<RefreshTokenResult> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new { RefreshToken = refreshToken };
            var response = await HttpClient.PostAsJsonAsync(
                RefreshEndpoint, 
                request, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                // API returns { Token, RefreshToken } - map to our RefreshTokenResult
                var apiResponse = await response.Content.ReadFromJsonAsync<RefreshTokenApiResponse>(
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                    cancellationToken);

                if (apiResponse != null && 
                    !string.IsNullOrWhiteSpace(apiResponse.Token) &&
                    !string.IsNullOrWhiteSpace(apiResponse.RefreshToken))
                {
                    return new RefreshTokenResult
                    {
                        Success = true,
                        AccessToken = apiResponse.Token, // Map Token to AccessToken
                        RefreshToken = apiResponse.RefreshToken
                    };
                }
            }

            return new RefreshTokenResult { Success = false };
        }
        catch
        {
            // Return failure result on any exception
            return new RefreshTokenResult { Success = false };
        }
    }

    /// <summary>
    /// Internal class to deserialize API refresh token response.
    /// </summary>
    private class RefreshTokenApiResponse
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}

