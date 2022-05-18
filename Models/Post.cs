using System;
using System.Collections.Generic;

namespace Life_API.Models
{
    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Fullname { get; set; }
        public DateTime? BirthYear { get; set; }
        public DateTime? DeathYear { get; set; }
        public string ImageURL { get; set; }
        public string Description { get; set; }
        public string Password { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? DeletedAt { get; set; }

        public ICollection<Comment> Comments { get; set; }

        //Relationship
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
