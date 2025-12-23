using Microsoft.AspNetCore.Identity;

namespace FAP.Common.Infrastructure.EntitiesMaster
{
    public class ApplicationUserClaim : IdentityUserClaim<string>
    {
        public string CampusCode { get; set; } = string.Empty;
    }
}

