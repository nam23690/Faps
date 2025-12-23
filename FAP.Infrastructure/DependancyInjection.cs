using FAP.API.Backend.Services;
using FAP.Common.Application.Interfaces;
using FAP.Common.Domain.Academic.Terms.Services;
using FAP.Common.Infrastructure.EntitiesMaster;
using FAP.Common.Infrastructure.Files;
using FAP.Common.Infrastructure.Persistence;
using FAP.Common.Infrastructure.Repositories;
using FAP.Common.Infrastructure.Services;
using FAP.Infrastructure.Persistence;
using FAP.Infrastructure.Seeders;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FAP.Common.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration config)
        {
            // =====================================================
            // CACHE (BẮT BUỘC – FIX TOÀN BỘ AggregateException)
            // =====================================================
            var redisConnectionString = config.GetConnectionString("Redis");

            if (!string.IsNullOrWhiteSpace(redisConnectionString))
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = redisConnectionString;
                });
            }
            else
            {
                services.AddDistributedMemoryCache(); // ❗ BẮT BUỘC
            }

            // =====================================================
            // COMMON SERVICES
            // =====================================================
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<ICampusProvider, CampusProvider>();
            services.AddScoped<IRequestMetadataService, RequestMetadataService>();
            services.AddScoped<IPermissionCacheService, PermissionCacheService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<IExcelService, ExcelService>();



            // =====================================================
            // DB CONTEXTS
            // =====================================================
            services.AddDbContext<UniversityDbContext>((sp, options) =>
            {
                var campusProvider = sp.GetRequiredService<ICampusProvider>();
                var connectionString = campusProvider.GetCampusConnectionString();
                options.UseSqlServer(connectionString);
            });

            services.AddScoped<IUniversityDbContext>(sp =>
                sp.GetRequiredService<UniversityDbContext>());

            services.AddDbContext<MasterDbContext>(options =>
            {
                options.UseSqlServer(config.GetConnectionString("MasterDb"));
            });

            services.AddDbContext<LoggingDbContext>(options =>
            {
                options.UseSqlServer(config.GetConnectionString("Logging"));
            });

            services.AddScoped<ILoggingDbContext>(sp =>
                sp.GetRequiredService<LoggingDbContext>());

            services.AddScoped<ICampusDbContextFactory, CampusDbContextFactory>();

            // =====================================================
            // IDENTITY
            // =====================================================
            services
                .AddIdentityCore<ApplicationUser>(options =>
                {
                    options.User.RequireUniqueEmail = true;
                    options.Password.RequiredLength = 6;
                })
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<MasterDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<UserManager<ApplicationUser>>();
            services.AddScoped<RoleManager<ApplicationRole>>();

            // =====================================================
            // DDD – REPOSITORIES (EXPLICIT)
            // =====================================================
            services.AddScoped<ITermRepository, TermRepository>();
            //Auto DI for Repositories
            services.Scan(scan => scan
               .FromAssemblyOf<InfrastructureAssemblyMarker>()     // chỉ scan assembly này
               .AddClasses(classes => classes.InNamespaces("FAP.Common.Infrastructure.Repositories"))
                   .AsMatchingInterface()                           // match I{ClassName}
                   .WithScopedLifetime());

            // =====================================================
            // DOMAIN SERVICES
            // =====================================================
            services.AddScoped<ITermUniquenessChecker, TermUniquenessChecker>();

            // =====================================================
            // UNIT OF WORK
            // =====================================================
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // =====================================================
            // SEEDERS
            // =====================================================
            services.AddScoped<MasterDbSeeder>();

            return services;
        }
    }
}