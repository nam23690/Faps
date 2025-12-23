using System.ComponentModel.DataAnnotations;

namespace FAP.API.Backend.Models
{

    public class ImportUserRequest
    {
        [Required]
        public IFormFile File { get; set; } = null!;
    }

}
