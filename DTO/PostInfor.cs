using System;

namespace Life_API.DTO
{
    public class PostInfor
    {
        public int PostId { get; set; }
        public string Fullname { get; set; }
        public DateTime? BirthYear { get; set; }
        public DateTime? DeathYear { get; set; }
        public DateTime CreatedAt { get; set; }
        public int NoComment { get; set; }
        public string ImageURL { get; set; }
    }
}
