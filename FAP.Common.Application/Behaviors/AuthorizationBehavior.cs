using FAP.Common.Application.Attributes;
using FAP.Common.Application.Exceptions;
using FAP.Common.Application.Interfaces;
using MediatR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FAP.Common.Application.Behaviors
{
    public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ICurrentUserService _currentUserService;

        public AuthorizationBehavior(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public async Task<TResponse> Handle(
            TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // Lấy tất cả FapPermissionAttribute trên request (AllowMultiple = true)
            var attrs = typeof(TRequest)
                .GetCustomAttributes(typeof(FapPermissionAttribute), false)
                .Cast<FapPermissionAttribute>()
                .ToList();

            // Mỗi attribute là một nhóm OR; các attribute chồng lên nhau là AND
            foreach (var attr in attrs)
            {
                var hasAny = false;
                foreach (var code in attr.Codes)
                {
                    if (await _currentUserService.HasPermissionAsync(code))
                    {
                        hasAny = true;
                        break;
                    }
                }

                if (!hasAny)
                {
                    throw new ForbiddenAccessException(
                        $"Bạn không có quyền. Cần một trong: {string.Join(", ", attr.Codes)}");
                }
            }

            return await next();
        }
    }
}
