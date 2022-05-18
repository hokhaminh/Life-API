using Life_API.Models;
using System;
using System.Collections.Generic;

namespace Life_API.DTO
{
    public class PostDetailResponseDTO
    {
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Fullname { get; set; }
        public DateTime? BirthYear { get; set; }
        public DateTime? DeathYear { get; set; }
        public string ImageURL { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }


        public ICollection<Comment> Comments { get; set; }
    }
}
