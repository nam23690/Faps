using System.Reflection;
using FAP.Common.Application.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
//using FAP.Common.Application.Behaviors; // Add this using directive if ValidationBehavior<,> is defined here

namespace FAP.Common.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // 1️⃣ Đăng ký MediatR: quét tất cả các Command / Query Handler trong assembly Application
        
            
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });

            // 2️⃣ Đăng ký FluentValidation
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // 3️⃣ Đăng ký AutoMapper (nếu dùng)
            //services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // 4️⃣ Đăng ký Pipeline Behaviors (nếu có)
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuditLoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
            
            return services;
        }
    }
}
