using System.ComponentModel.DataAnnotations;

namespace FAP.Share.Dtos
{
    public class RoleDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? NormalizedName { get; set; }
        public int UserCount { get; set; }
    }

    public class AssignRoleRequest
    {
        public string UserId { get; set; } = string.Empty;
        public List<string> RoleNames { get; set; } = new();
        public string? CampusCode { get; set; }
    }


    /// <summary>
    /// View model for creating a user
    /// </summary>
    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        [Display(Name = "Tên đăng nhập")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        [Display(Name = "Mật khẩu")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Mã cơ sở")]
        public string CampusId { get; set; } = string.Empty;

        [Display(Name = "URL ảnh đại diện")]
        public string? ProfilePictureUrl { get; set; }

        [Display(Name = "Vai trò")]
        public List<string>? SelectedRoles { get; set; }

        [Display(Name = "Mã cơ sở")]
        public string? CampusCode { get; set; }
    }

    /// <summary>
    /// View model for editing a user
    /// </summary>
    public class EditUserViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Mã cơ sở")]
        public string CampusId { get; set; } = string.Empty;

        [Display(Name = "URL ảnh đại diện")]
        public string? ProfilePictureUrl { get; set; }

        [Display(Name = "Khóa tài khoản")]
        public bool IsLocked { get; set; }
    }

    /// <summary>
    /// View model for assigning roles to user
    /// </summary>
    public class AssignRolesViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public List<string> CurrentRoles { get; set; } = new();
        public List<string> AvailableRoles { get; set; } = new();
        public List<string>? SelectedRoles { get; set; }
        public string? CampusCode { get; set; }
    }

    /// <summary>
    /// View model for assigning permissions to user
    /// </summary>
    public class AssignPermissionsViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public List<string> CurrentPermissions { get; set; } = new();
        public List<PermissionDto> AvailablePermissions { get; set; } = new();
        public List<string>? SelectedPermissions { get; set; }
        public string? CampusCode { get; set; }
    }

    /// <summary>
    /// View model for assigning permissions to role
    /// </summary>
    public class AssignPermissionsToRoleViewModel
    {
        public string RoleId { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public List<string> CurrentPermissions { get; set; } = new();
        public List<PermissionDto> AvailablePermissions { get; set; } = new();
        public List<string>? SelectedPermissions { get; set; }
    }

    /// <summary>
    /// View model for creating a role
    /// </summary>
    public class CreateRoleViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên vai trò")]
        [Display(Name = "Tên vai trò")]
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// View model for editing a role
    /// </summary>
    public class EditRoleViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập tên vai trò")]
        [Display(Name = "Tên vai trò")]
        public string Name { get; set; } = string.Empty;
    }


}

