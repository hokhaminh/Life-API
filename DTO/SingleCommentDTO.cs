using System;

namespace Life_API.DTO
{
    public class SingleCommentDTO
    {
        public int CommentId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string CommentContent { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
