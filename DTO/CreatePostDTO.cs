using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace Life_API.DTO
{
    public class CreatePostDTO
    {
        [Required]
        [StringLength(70, MinimumLength = 6)]
        public string Title { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Fullname { get; set; }

        [Required]
        public DateTime? BirthYear { get; set; }

        public DateTime? DeathYear { get; set; }

        public IFormFile? Image { get; set; }

        [Required]
        [StringLength(550, MinimumLength = 20)]
        public string Description { get; set; }

        [StringLength(20, MinimumLength = 3)]
        public string? Password { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public int UserId { get; set; }
    }
}
