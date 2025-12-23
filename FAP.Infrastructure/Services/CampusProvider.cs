using FAP.Common.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAP.Common.Infrastructure.Services
{
    public class CampusProvider : ICampusProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICurrentUserService _currentUserService;
        private readonly IConfiguration _config;
        public CampusProvider(IHttpContextAccessor accessor, ICurrentUserService currentUserService, IConfiguration config)
        {
            _httpContextAccessor = accessor;
            _currentUserService = currentUserService;
            _config = config;
        }

        public string GetCampusCode()
        {
            if(!string.IsNullOrEmpty(_currentUserService.CampusCode))
            {
                return _currentUserService.CampusCode;
            }
            return _httpContextAccessor.HttpContext?.Items["Campus"]?.ToString();
        }

        public string GetCampusConnectionString()
        {
            var campusCode = GetCampusCode();            
            return _config.GetConnectionString(campusCode);
        }
    }

}
