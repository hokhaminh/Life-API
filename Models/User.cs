using System;
using System.Collections.Generic;

namespace Life_API.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; } = false;
        public string Phone { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime DeactivatedAt { get; set; }
        public string Role { get; set; }

        public ICollection<Post> Posts { get; set; }
    }
}
