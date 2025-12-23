using FAP.Share.Dtos;
using FAP.Shared.Http.Clients;

namespace FAP.Admin.Web.Services;

/// <summary>
/// API client for authentication operations.
/// </summary>
public class AuthApiClient : ApiClientBase, IAuthApiClient
{
    private const string LoginEndpoint = "/api/auth/login";
    private const string LoginFeIdEndpoint = "/api/auth/loginfeid";

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthApiClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client.</param>
    public AuthApiClient(HttpClient httpClient)
        : base(httpClient)
    {
    }

    /// <inheritdoc/>
    public async Task<LoginResponse?> LoginAsync(UserLoginRequest request)
    {
        return await PostAsync<UserLoginRequest, LoginResponse>(LoginEndpoint, request);
    }

    public async Task<LoginResponse?> LoginFeIdAsync(LoginFeIdRequest request)
    {
        return await PostAsync<LoginFeIdRequest, LoginResponse>(LoginFeIdEndpoint, request);
    }
}

