using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAP.Share.Dtos
{
    public class UserLoginRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn cơ sở")]
        [Display(Name = "Cơ sở (Campus)")]
        public string CampusCode { get; set; } = string.Empty;
    }


    /// <summary>
    /// Response model from login API call.
    /// Matches FAP.API.Backend LoginResponse structure.
    /// </summary>

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
        public List<string> Permissions { get; set; } = new();
        public string CampusCode { get; set; } = string.Empty;
    }


    public class LoginFeIdRequest
    {
        [Required(ErrorMessage = "AccessToken Không được trống")]
        public string AccessToken { get; set; } = string.Empty;
        public string CampusCode { get; set; } = string.Empty;
    }

}
