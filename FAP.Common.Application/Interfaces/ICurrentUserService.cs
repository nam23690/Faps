using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAP.Common.Application.Interfaces
{
    
        public interface ICurrentUserService
        {
            string? UserId { get; }
            string? UserName { get; }
            string? Email { get; }
            bool IsAuthenticated { get; }
            string? CampusCode { get; }
            Task<bool> HasPermissionAsync(string permission);
            }
    

}
