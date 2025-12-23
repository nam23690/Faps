using FAP.Shared.Http.Abstractions;
using FAP.Shared.Http.Clients;

namespace FAP.Admin.Web.Services;

/// <summary>
/// Example API client demonstrating how to use the shared HTTP infrastructure.
/// This can be replaced with actual API clients for your domain.
/// </summary>
public interface IExampleApiClient : IApiClient
{
    // Add specific methods here as needed
    // Example: Task<UserDto?> GetUserAsync(int userId);
}

/// <summary>
/// Example implementation of an API client.
/// </summary>
public class ExampleApiClient : ApiClientBase, IExampleApiClient
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExampleApiClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client.</param>
    public ExampleApiClient(HttpClient httpClient)
        : base(httpClient)
    {
    }

    // Add specific methods here as needed
    // Example:
    // public async Task<UserDto?> GetUserAsync(int userId)
    // {
    //     return await GetAsync<UserDto>($"/api/users/{userId}");
    // }
}

