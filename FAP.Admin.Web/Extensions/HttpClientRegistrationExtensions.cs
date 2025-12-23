using System.Reflection;
using FAP.Shared.Http.Abstractions;
using FAP.Shared.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace FAP.Admin.Web.Extensions;

public static class HttpClientRegistrationExtensions
{
    /// <summary>
    /// Scan given assemblies for concrete types that implement an interface derived from <see cref="IApiClient"/>
    /// and register them with <see cref="FAP.Shared.Http.Extensions.ServiceCollectionExtensions.AddAuthenticatedHttpClient{TClient,TImplementation}"/>.
    /// </summary>
    public static IServiceCollection AddAllAuthenticatedHttpClients(this IServiceCollection services, string baseAddress, params Assembly[] assemblies)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (string.IsNullOrWhiteSpace(baseAddress)) throw new ArgumentNullException(nameof(baseAddress));

        // Get reference to the generic helper from FAP.Shared.Http.Extensions
        var helperType = typeof(FAP.Shared.Http.Extensions.ServiceCollectionExtensions);
        var helperMethod = helperType.GetMethod("AddAuthenticatedHttpClient", BindingFlags.Public | BindingFlags.Static);
        if (helperMethod == null)
            throw new InvalidOperationException("Unable to find AddAuthenticatedHttpClient helper method.");

        foreach (var asm in assemblies)
        {
            var types = asm.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsPublic);
            foreach (var impl in types)
            {
                // find interfaces implemented by impl which inherit IApiClient
                var apiInterfaces = impl.GetInterfaces().Where(i => typeof(IApiClient).IsAssignableFrom(i) && i.IsInterface);
                foreach (var iface in apiInterfaces)
                {
                    var generic = helperMethod.MakeGenericMethod(iface, impl);
                    generic.Invoke(null, new object[] { services, baseAddress });
                }
            }
        }

        return services;
    }
}
