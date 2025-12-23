using FAP.API.Backend;
using FAP.Common.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net.Http.Headers;


namespace FAP.Test.Integration
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Xóa DbContext gốc
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<UniversityDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                // Dùng InMemory DB
                services.AddDbContext<UniversityDbContext>(options =>
                {
                    options.UseInMemoryDatabase("IntegrationTestDB");
                });
            });
        }
        public HttpClient CreateClientWithJwt(string token)
        {
            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }
    }
}
