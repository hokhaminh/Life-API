using Microsoft.AspNetCore.Http;
using System;

namespace Life_API.DTO
{
    public class UpdatePostDTO
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Fullname { get; set; }
        public DateTime? BirthYear { get; set; }
        public DateTime? DeathYear { get; set; }
        public IFormFile Image { get; set; }
        public string Description { get; set; }
        public string Password { get; set; }
    }
}
