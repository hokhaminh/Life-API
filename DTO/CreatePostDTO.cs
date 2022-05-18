using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace Life_API.DTO
{
    public class CreatePostDTO
    {
        [Required]
        //[StringLength(20, MinimumLength = 6)]
        public string Title { get; set; }

        [Required]
        //[StringLength(20, MinimumLength = 3)]
        public string Fullname { get; set; }

        [Required]
        public DateTime BirthYear { get; set; }


        public DateTime? DeathYear { get; set; }

        [Required]
        public IFormFile Image { get; set; }

        [Required]
        //[StringLength(200, MinimumLength = 20)]
        public string Description { get; set; }

        [Required]
        //[StringLength(20, MinimumLength = 3)]
        public string Password { get; set; }

        [Required]
        public int UserId { get; set; }
    }
}
