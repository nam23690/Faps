using FAP.Common.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAP.Common.Infrastructure.Services
{
    public class RequestMetadataService : IRequestMetadataService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RequestMetadataService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string UserId => _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;
        public string CampusCode => _httpContextAccessor.HttpContext?.Items["CampusCode"]?.ToString();
        public string CorrelationId => _httpContextAccessor.HttpContext?.TraceIdentifier;
        public string IpAddress => _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
        public string UserAgent => _httpContextAccessor.HttpContext?.Request?.Headers["User-Agent"].ToString();
        public string Method => _httpContextAccessor.HttpContext?.Request?.Method;
        public string Path => _httpContextAccessor.HttpContext?.Request?.Path;
        public int StatusCode
        {
            get
            {
                var ctx = _httpContextAccessor.HttpContext;
                return ctx?.Response?.StatusCode ?? 0;
            }
        }
        
    }

}
