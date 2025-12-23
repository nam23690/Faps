using System.Net;
using System.Text.Json;
using FAP.Common.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FAP.Infrastructure.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (UnauthorizedException ex)
            {
                _logger.LogWarning(ex, "Unauthorized");
                await WriteError(context, HttpStatusCode.Unauthorized, ex.Message);
            }
            catch (ForbiddenAccessException ex)
            {
                _logger.LogWarning(ex, "Forbidden");
                await WriteError(context, HttpStatusCode.Forbidden, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                await WriteError(context, HttpStatusCode.InternalServerError,
                    "Đã có lỗi hệ thống xảy ra.");
            }
        }

        private static async Task WriteError(
            HttpContext context,
            HttpStatusCode status,
            string message)
        {
            context.Response.StatusCode = (int)status;
            context.Response.ContentType = "application/json";

            var payload = new { message };
            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
    }
}
