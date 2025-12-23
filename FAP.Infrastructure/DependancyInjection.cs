using FAP.API.Backend.Services;
using FAP.Common.Application.Interfaces;
using FAP.Common.Infrastructure.Files;
using FAP.Common.Infrastructure.Persistence;
using FAP.Common.Infrastructure.Services;
using FAP.Infrastructure.Persistence;
using FAP.Infrastructure.Seeders;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FAP.Common.Infrastructure.EntitiesMaster;


namespace FAP.Common.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            //services.AddDbContext<UniversityDbContext>(options =>
            //  options.UseSqlServer(config.GetConnectionString("DefaultConnection")));
            
            // Cấu hình Redis cache
            var redisConnectionString = config.GetConnectionString("Redis");
            if (!string.IsNullOrEmpty(redisConnectionString))
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = redisConnectionString;
                });
            }
            else
            {
                // Fallback to memory cache nếu không có Redis
                services.AddDistributedMemoryCache();
            }

            // Đăng ký PermissionCacheService
            services.AddScoped<IPermissionCacheService, PermissionCacheService>();
            
            // Đăng ký RefreshTokenService
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            
            services.AddDbContext<LoggingDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("Logging")));
            /*
            services.AddTransient<IAuditLogRepository, AuditLogRepository>();

           
            services.AddTransient<IPerformanceLogRepository, PerformanceLogRepository>();
            */
            services.AddScoped<IRequestMetadataService, RequestMetadataService>();
            // Đăng ký các repository, service khác
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<ICampusProvider, CampusProvider>();
            services.AddDbContext<UniversityDbContext>((serviceProvider, optionsBuilder) =>
            {
                var campusProvider = serviceProvider.GetRequiredService<ICampusProvider>();
                var connStr = campusProvider.GetCampusConnectionString();
                optionsBuilder.UseSqlServer(connStr);
            });
            //services.AddScoped<IUniversityDbContext>(provider => provider.GetRequiredService<UniversityDbContext>());
            //services.AddDbContext<IUniversityDbContext, UniversityDbContext>();
            
            services.AddScoped<ILoggingDbContext>(provider => provider.GetRequiredService<LoggingDbContext>());
            services.AddScoped<ICampusDbContextFactory, CampusDbContextFactory>();

            //Auto DI for Repositories
            services.Scan(scan => scan
               .FromAssemblyOf<InfrastructureAssemblyMarker>()     // chỉ scan assembly này
               .AddClasses(classes => classes.InNamespaces("FAP.Common.Infrastructure.Repositories"))
                   .AsMatchingInterface()                           // match I{ClassName}
                   .WithScopedLifetime()
             );
            //services.AddScoped<ITermRepository, TermRepository>();
            //services.AddScoped<IUserRepository, UserRepository>();

            services.AddDbContext<MasterDbContext>(options =>
            {
                options.UseSqlServer(config.GetConnectionString("MasterDb"));
            });

            services
                .AddIdentityCore<ApplicationUser>(options =>
                {
                    options.User.RequireUniqueEmail = true;
                    options.Password.RequiredLength = 6;
                })
                .AddRoles<ApplicationRole>() // nếu dùng role
                .AddEntityFrameworkStores<MasterDbContext>()
                .AddDefaultTokenProviders();

            // Đăng ký UserManager và RoleManager để sử dụng trong Application layer
            services.AddScoped<UserManager<ApplicationUser>>();
            services.AddScoped<RoleManager<ApplicationRole>>();

            services.AddScoped<IExcelService, ExcelService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            
            // Đăng ký Seeder
            services.AddScoped<MasterDbSeeder>();
            
            return services;

        }
    }
}
