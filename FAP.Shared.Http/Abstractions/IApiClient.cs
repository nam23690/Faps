using System.Net.Http;

namespace FAP.Shared.Http.Abstractions;

/// <summary>
/// Base interface for API clients that provides common HTTP operations.
/// This interface follows the Interface Segregation Principle by focusing 
/// on essential HTTP operations without coupling to specific implementations.
/// </summary>
public interface IApiClient
{
    /// <summary>
    /// Performs a GET request and deserializes the response.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the response to.</typeparam>
    /// <param name="requestUri">The URI to request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The deserialized response object.</returns>
    Task<T?> GetAsync<T>(string requestUri, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs a POST request with a request body and deserializes the response.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request body.</typeparam>
    /// <typeparam name="TResponse">The type to deserialize the response to.</typeparam>
    /// <param name="requestUri">The URI to request.</param>
    /// <param name="request">The request body object.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The deserialized response object.</returns>
    Task<TResponse?> PostAsync<TRequest, TResponse>(string requestUri, TRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs a POST request with a request body without expecting a response body.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request body.</typeparam>
    /// <param name="requestUri">The URI to request.</param>
    /// <param name="request">The request body object.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The HTTP response message.</returns>
    Task<HttpResponseMessage> PostAsync<TRequest>(string requestUri, TRequest request, CancellationToken cancellationToken = default);
}

