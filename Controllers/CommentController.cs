using Life_API.DTO;
using Life_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Life_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        public readonly AppDBContext db;

        public CommentController(AppDBContext db)
        {
            this.db = db;
        }
        [HttpPost("create")]
        [AllowAnonymous]
        public IActionResult CreateComment(CreateCommentDTO NewCommentInput)
        {
            var PostFound = db.Posts.Find(NewCommentInput.PostId);
            if (PostFound == null)
            {
                return NotFound(new ResponseDTO(404, "This post is not found"));
            }
            else if (PostFound.DeletedAt != null)
            {
                return NotFound(new ResponseDTO(404, "This post is deleted"));
            }

            var NewComment = new Comment
            {
                CommentContent = NewCommentInput.CommentContent,
                Name = NewCommentInput.Name,
                Email = NewCommentInput.Email,
                CreatedAt = DateTime.Now,
                PostId = NewCommentInput.PostId,
            };

            db.Comments.Add(NewComment);
            db.SaveChanges();
            return Created("", new ResponseDTO(201, "Insert comment successfully"));
        }

        [HttpGet("{id:int}")]
        //[Authorize(Roles = "User")]
        public IActionResult GetCommentList(int id)
        {
            Post PostFound = db.Posts.Find(id);
            if(PostFound == null)
            {
                return NotFound(new ResponseDTO(404, "Post not found"));
            }
            else if (PostFound.DeletedAt != null)
            {
                return NotFound(new ResponseDTO(404, "Post was deleted"));
            }

            var commentBigList = db.Comments.Where(m => m.PostId == id).ToList();
            var commentList = new List<SingleCommentDTO>();

            foreach (var commentBig in commentBigList)
            {
                commentList.Add(new SingleCommentDTO
                {
                    CommentContent= commentBig.CommentContent,
                    CommentId = commentBig.CommentId,
                    Name = commentBig.Name,
                    Email= commentBig.Email,
                    CreatedAt=commentBig.CreatedAt
                });
            }
            var commentListResponse = new CommentListResponseDTO
            {
                PostId = id,
                CommentList = commentList
            };
            
            return Ok(commentList);
        }
    }
}
