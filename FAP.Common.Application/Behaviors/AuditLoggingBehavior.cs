using FAP.Common.Application.Attributes;
using FAP.Common.Application.Interfaces;
using FAP.Common.Domain.Entities;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FAP.Common.Application.Behaviors
{
    public class AuditLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly IAuditLogRepository _auditLogRepo;
        private readonly IRequestMetadataService _metadata;
        private readonly IServiceProvider _serviceProvider;
        public AuditLoggingBehavior(
            ICurrentUserService currentUser,
            IAuditLogRepository auditLogRepo,
            IRequestMetadataService metadata,
            IServiceProvider serviceProvider)
        {
            _currentUser = currentUser;
            _auditLogRepo = auditLogRepo;
            _metadata = metadata;
            _serviceProvider = serviceProvider;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            string? oldValue = null;
          /*  string? entityName = ExtractEntityName(request);
            string? entityId = ExtractEntityId(request);

            // Step 1: Before -> lấy dữ liệu cũ nếu có Id
            if (!string.IsNullOrEmpty(entityId))
            {
                var repo = ResolveRepository(entityName);
                if (repo != null)
                {
                    var oldEntity = await repo.GetByIdAsync(int.Parse(entityId),cancellationToken);
                    if (oldEntity != null)
                    {
                        oldValue = System.Text.Json.JsonSerializer.Serialize(oldEntity);
                    }
                }
            }
          */
            // Thời điểm thực hiện lệnh
            var response = await next();

            var log = new AuditLog
            {
                UserId = _currentUser.UserId,
                Username = _currentUser.UserName,
                Action = typeof(TRequest).Name, // tên command
                Entity = ExtractEntityName(request),
                EntityId = ExtractEntityId(request),
                CampusCode = _currentUser.CampusCode,
                IpAddress = _metadata.IpAddress,
                OldValue = oldValue,
                NewValue = System.Text.Json.JsonSerializer.Serialize(request),
                CreatedAt = DateTime.UtcNow
            };

            await _auditLogRepo.AddAsync(log);

            return response;
        }

       // private string ExtractEntityName(TRequest request)
        //    => request.GetType().Name.Replace("Command", "").Replace("Query", "");

        private string ExtractEntityName(TRequest request)
        {
            var attr = request.GetType().GetCustomAttribute<AuditEntityAttribute>();
            if (attr != null)
                return attr.EntityName;

            // fallback regex nếu không có attribute
            return FallbackExtractEntityName(request.GetType().Name);
        }

        private static string FallbackExtractEntityName(string requestTypeName)
        {
            // Ví dụ: CreateUserCommand -> User
            //        UpdateUserCommand -> User
            //        DeleteUserCommand -> User
            string[] suffixes = { "Create", "Update", "Delete", "Get", "Command", "Query", "ById" };

            foreach (var suffix in suffixes)
            {
                requestTypeName = requestTypeName.Replace(suffix, "");
            }

            return requestTypeName;
        }
        private string ExtractEntityId(TRequest request)
        {
            var prop = request.GetType().GetProperty("Id");
            return prop?.GetValue(request)?.ToString();
        }

        private dynamic? ResolveRepository(string entityName)
        {
            // convention: User -> IUserRepository
            var repoName = $"I{entityName}Repository";
            var repoInterface = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.Name == repoName);

            if (repoInterface == null)
                return null;

            return _serviceProvider.GetService(repoInterface);
        }
    }

}
