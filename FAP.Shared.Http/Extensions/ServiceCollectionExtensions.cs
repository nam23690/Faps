using System.Net.Http.Headers;
using FAP.Shared.Http.Abstractions;
using FAP.Shared.Http.Clients;
using FAP.Shared.Http.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace FAP.Shared.Http.Extensions;

/// <summary>
/// Extension methods for registering HTTP client services.
/// Provides a clean, centralized way to configure all HTTP client dependencies.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds shared HTTP client services including JWT handler and authentication API client.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="baseAddress">The base address for the API.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when services or baseAddress is null.</exception>
    public static IServiceCollection AddSharedHttpClients(
        this IServiceCollection services,
        string baseAddress)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));
        if (string.IsNullOrWhiteSpace(baseAddress))
            throw new ArgumentNullException(nameof(baseAddress));

        // Register JWT delegating handler as transient (per-request)
        services.AddTransient<JwtDelegatingHandler>();

        // Register AuthApiClient with HttpClient (without JwtDelegatingHandler to avoid circular dependency)
        // The refresh endpoint should not require authentication
        services.AddHttpClient<IAuthApi, AuthApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        });

        return services;
    }

    /// <summary>
    /// Adds a typed HTTP client with JWT authentication support.
    /// </summary>
    /// <typeparam name="TClient">The client interface type.</typeparam>
    /// <typeparam name="TImplementation">The client implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="baseAddress">The base address for the API.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAuthenticatedHttpClient<TClient, TImplementation>(
        this IServiceCollection services,
        string baseAddress)
        where TClient : class
        where TImplementation : class, TClient
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));
        if (string.IsNullOrWhiteSpace(baseAddress))
            throw new ArgumentNullException(nameof(baseAddress));

        services.AddHttpClient<TClient, TImplementation>(client =>
        {
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        })
        .AddHttpMessageHandler<JwtDelegatingHandler>();

        return services;
    }
}

