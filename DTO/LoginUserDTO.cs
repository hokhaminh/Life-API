using System.ComponentModel.DataAnnotations;

namespace Life_API.DTO
{
    public class LoginUserDTO
    {
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
