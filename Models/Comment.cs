using System;

namespace Life_API.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string CommentContent { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        //Relationship
        public int PostId { get; set; }
        public Post Post { get; set; }
    }
}
