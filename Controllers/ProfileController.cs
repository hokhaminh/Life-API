using Life_API.DTO;
using Life_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Life_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        public readonly AppDBContext db;

        public ProfileController(AppDBContext db)
        {
            this.db = db;
        }

        [HttpGet("{id:int}")]
        //[Authorize(Roles = "User")]
        public IActionResult ProfileDetail(int id)
        {
            //find user by id
            var userFound = db.Users.Where(m=> m.DeactivatedAt == default && m.UserId == id).FirstOrDefault();

            //Find the post belong to that user
            var BigPostList = db.Posts.Where(m => m.UserId == id && m.DeletedAt == null).ToList();
            
            //create a Post list with many posts information in it
            var PostList = new List<PostInfor>();

            foreach (var post in BigPostList)
            {
                PostList.Add(new PostInfor()
                {
                    PostId = post.PostId,
                    ImageURL = post.ImageURL,
                    Fullname = post.Fullname,
                    BirthYear = post.BirthYear,
                    DeathYear = post.DeathYear,
                    CreatedAt = post.CreatedAt,
                    NoComment = db.Comments.Where(m => m.PostId == post.PostId).Count()
                });
            }

            //the main response
            var ProfileRespose = new ProfileResponseDTO
            {
                UserId = userFound.UserId,
                Fullname = userFound.Fullname,
                UserName = userFound.UserName,
                Email = userFound.Email,
                Phone = userFound.Phone,
                CreatedAt = userFound.CreatedAt,
                Posts = PostList
            };

            return Ok(ProfileRespose);
        }
    }
}
