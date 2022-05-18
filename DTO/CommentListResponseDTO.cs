using Life_API.Models;
using System.Collections.Generic;

namespace Life_API.DTO
{
    public class CommentListResponseDTO
    {
        public int PostId { get; set; }
        public List<SingleCommentDTO> CommentList { get; set; }
    }
}
