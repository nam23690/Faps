using Microsoft.AspNetCore.Identity;

namespace FAP.Common.Infrastructure.EntitiesMaster
{
    public class ApplicationUserRole : IdentityUserRole<string>
    {
        public string CampusCode { get; set; } = string.Empty;
    }
}

