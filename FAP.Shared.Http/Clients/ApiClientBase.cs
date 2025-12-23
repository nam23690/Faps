using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FAP.Shared.Http.Abstractions;

namespace FAP.Shared.Http.Clients;

/// <summary>
/// Base class for API clients providing common HTTP operations.
/// Implements the Template Method pattern to provide consistent error handling
/// and response deserialization across all API clients.
/// </summary>
public abstract class ApiClientBase : IApiClient
{
    /// <summary>
    /// Gets the HTTP client instance.
    /// </summary>
    protected readonly HttpClient HttpClient;

    /// <summary>
    /// Gets the JSON serializer options.
    /// </summary>
    protected readonly JsonSerializerOptions JsonOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiClientBase"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client to use for requests.</param>
    protected ApiClientBase(HttpClient httpClient)
    {
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    /// <inheritdoc/>
    public virtual async Task<T?> GetAsync<T>(string requestUri, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.GetAsync(requestUri, cancellationToken);
        
        await EnsureSuccessStatusCodeAsync(response);
        
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async Task<TResponse?> PostAsync<TRequest, TResponse>(
        string requestUri, 
        TRequest request, 
        CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PostAsJsonAsync(requestUri, request, JsonOptions, cancellationToken);
        
        await EnsureSuccessStatusCodeAsync(response);
        
        return await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async Task<System.Net.Http.HttpResponseMessage> PostAsync<TRequest>(
        string requestUri, 
        TRequest request, 
        CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PostAsJsonAsync(requestUri, request, JsonOptions, cancellationToken);
        
        await EnsureSuccessStatusCodeAsync(response);
        
        return response;
    }

    /// <summary>
    /// Ensures the response has a success status code, throwing an exception if not.
    /// </summary>
    /// <param name="response">The HTTP response message.</param>
    /// <exception cref="HttpRequestException">Thrown when the response is not successful.</exception>
    protected virtual async Task EnsureSuccessStatusCodeAsync(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            var exception = new HttpRequestException(
                $"Request failed with status code {response.StatusCode}. Response: {errorContent}",
                null,
                response.StatusCode);
            
            // Store status code in exception data for easier handling
            exception.Data["StatusCode"] = response.StatusCode;
            
            throw exception;
        }
    }
}

