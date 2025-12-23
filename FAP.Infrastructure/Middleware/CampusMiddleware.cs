using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//Có thể dùng nếu muốn lấy thông tin campus từ header của request, tuy nhiên hiện tại chưa sử dụng
namespace FAP.Common.Infrastructure.Middleware
{
    public class CampusMiddleware
    {
        private readonly RequestDelegate _next;

        public CampusMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var campusCode = context.Request.Headers["X-Campus-Code"].FirstOrDefault();
            context.Items["Campus"] = campusCode;
            await _next(context);
        }
    }

}
