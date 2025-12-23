using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAP.Share.Dtos
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Login { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
    }

    public class UserExportDto
    {
        public long Id { get; set; }
        public string UserName { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
    }

    public class IdentityUserDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string CampusId { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; }
        public bool IsLocked { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> Roles { get; set; } = new();
        public List<string> Permissions { get; set; } = new();
    }

    public class UserImportDto
    {
        public string Login { get; set; } = "";
        public string Fullname { get; set; } = "";
        public string Email { get; set; } = "";
    }
}
