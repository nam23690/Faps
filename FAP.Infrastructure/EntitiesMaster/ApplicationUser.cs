using Microsoft.AspNetCore.Identity;

namespace FAP.Common.Infrastructure.EntitiesMaster
{
    public class ApplicationUser : IdentityUser
    {
        public string CampusId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int IsLocked { get; set; }
        public string ProfilePictureUrl { get; set; } = string.Empty;
    }
}

