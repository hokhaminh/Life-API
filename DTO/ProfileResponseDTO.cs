using System;
using System.Collections.Generic;

namespace Life_API.DTO
{
    public class ProfileResponseDTO
    {
        public int UserId { get; set; }
        public string Fullname { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<PostInfor> Posts { get; set; }
    }
}
