using System.ComponentModel.DataAnnotations;

namespace Life_API.DTO
{
    public class CreateCommentDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string CommentContent { get; set; }
        [Required]
        public int PostId { get; set; }
    }
}
