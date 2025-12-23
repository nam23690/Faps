using FAP.Share.Dtos;

namespace FAP.Admin.Web.Services
{

    /// <summary>
    /// Interface for authentication API client.
    /// </summary>
    public interface IAuthApiClient
    {
        /// <summary>
        /// Performs login operation.
        /// </summary>
        /// <param name="request">The login request.</param>
        /// <returns>The login response.</returns>
        Task<LoginResponse?> LoginAsync(UserLoginRequest request);

        Task<LoginResponse?> LoginFeIdAsync(LoginFeIdRequest request);
    }
}


