using Life_API.DTO;
using Life_API.Helper;
using Life_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Life_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : ControllerBase
    {
        public readonly AppDBContext db;

        public string apiUrl = "https://api.imgbb.com/1/upload?key=1a757ca0e10490ce1d4b7574eea5343a";
        public string secretKey= "6LepWdUhAAAAAPdhyELjzfhNCKuNZ47bSs1vb9Wn";

        public PostController(AppDBContext db)
        {
            this.db = db;

        }

        [HttpPost("create")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult> CreatePost([FromForm] CreatePostDTO newPost)
        {
            //check if user is passed the captcha
            if (!await VerifyToken(newPost.Token))
            {
                return BadRequest(new ResponseDTO(404, "Robot detected, if not please try again later"));
            }
            string avatarUrl = null;
            //check if image is null
            if (newPost.Image != null)
            {
                (bool isImage, string errorMsg) = CheckImage.CheckImageExtension(newPost.Image);
                if (isImage == false)
                {
                return Conflict(new ResponseDTO(409, errorMsg));
                }

                string fileName = ContentDispositionHeaderValue.Parse(newPost.Image.ContentDisposition).FileName.Trim('"');
                Stream stream = newPost.Image.OpenReadStream();
                long fileLength = newPost.Image.Length;
                string fileType = newPost.Image.ContentType;

                avatarUrl = await Upload(stream, fileName, fileLength, fileType);
            }
            

            var newPostModel = new Post
            {
                Fullname = newPost.Fullname,
                BirthYear = newPost.BirthYear,
                DeathYear = newPost.DeathYear,
                Description = newPost.Description,
                ImageURL = avatarUrl,
                Password = newPost.Password,
                Title = newPost.Title,
                UserId = newPost.UserId,
            };
            db.Posts.Add(newPostModel);
            db.SaveChanges();

            return Created("", newPostModel.PostId);

        }

        [HttpGet("{id:int}")]
        public IActionResult GetPost(int id)
        {
            var postExisted = db.Posts.Any(p => p.PostId == id);
            if (postExisted == false)
            {
                return NotFound(new ResponseDTO(404, "Post not found"));
            }

            var PostFound = db.Posts.Where(p => p.PostId == id && p.DeletedAt == null).FirstOrDefault();
            if (PostFound == null)
            {
                return NotFound(new ResponseDTO(404, "This post was deleted"));
            }

            var PostResponse = new PostDetailResponseDTO
            {
                UserId = PostFound.UserId,
                Title = PostFound.Title,
                BirthYear = PostFound.BirthYear,
                DeathYear = PostFound.DeathYear,
                CreatedAt = PostFound.CreatedAt,
                Comments = PostFound.Comments,
                Description = PostFound.Description,
                Fullname = PostFound.Fullname,
                ImageURL = PostFound.ImageURL,
            };

            return Ok(PostResponse);
        }

        [HttpPost("password")]
        public IActionResult CheckPostPassword(PasswordCheckDTO passwordCheckDTO)
        {
            var PostFound = db.Posts.Find(passwordCheckDTO.PostId);
            if (PostFound == null)
            {
                return NotFound(new ResponseDTO(404, "Post not found"));
            }

            if (passwordCheckDTO.Password.Equals(PostFound.Password))
            {
                return Ok(true);
            }

            return Ok(false);
        }

        [HttpGet("password/{id:int}")]
        public IActionResult GetPostPassword(int id)
        {
            var PostFound = db.Posts.Find(id);
            if (PostFound == null)
            {
                return NotFound(new ResponseDTO(404, "Post not found"));
            }

            return Ok(PostFound.Password);
        }

        //Get post user id by post id
        [HttpGet("user/{id:int}")]
        public IActionResult GetPostUserId(int id)
        {
            var PostFound = db.Posts.Find(id);
            if (PostFound == null)
            {
                return NotFound(new ResponseDTO(404, "Post not found"));
            }

            return Ok(PostFound.UserId);
        }
        //Check if post password is null
        [HttpGet("password/null/{id:int}")]
        public IActionResult IsPostPasswordNull(int id)
        {
            var PostFound = db.Posts.Find(id);
            if (PostFound == null)
            {
                return NotFound(new ResponseDTO(404, "Post not found"));
            }

            if (PostFound.Password == null)
            {
                return Ok(true);
            }

            return Ok(false);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "User")]
        public IActionResult DeletePost(int id)
        {
            //check if Post is Existed
            var postExisted = db.Posts.Any(p => p.PostId == id);
            if (postExisted == false)
            {
                return NotFound(new ResponseDTO(404, "Post not found"));
            }

            //check if Post is Deleted 
            var postFound = db.Posts.Where(p => p.PostId == id && p.DeletedAt == null).FirstOrDefault();
            if(postFound == null)
            {
                return NotFound(new ResponseDTO(404, "This post was deleted"));
            }

            //Delete this post
            postFound.DeletedAt = DateTime.Now;

            var result = db.SaveChanges();

            return Ok(new ResponseDTO(200, "Delete Post success"));
        }

        [HttpPut("{id:int}/update")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult> UpdatePost(int id, [FromForm] UpdatePostDTO updatePostDTO)
        {
            //check if Post is Existed
            bool exited = CheckPostExited(updatePostDTO.PostId);
            if (exited == false) return NotFound(new ResponseDTO(404, "Post not found"));

            //check if Post is Deleted 
            var postFound = CheckPostDeleted(updatePostDTO.PostId);
            if (postFound == null) return NotFound(new ResponseDTO(404, "This post was deleted"));

            //check if user is the owner of the post
            //if (postFound.UserId != id) return NotFound(new ResponseDTO(403, "You are not allowed to update this post"));

            //update a record in Post
            if(updatePostDTO.Image != null)
            {
                string fileName = ContentDispositionHeaderValue.Parse(updatePostDTO.Image.ContentDisposition).FileName.Trim('"');
                Stream stream = updatePostDTO.Image.OpenReadStream();
                long fileLength = updatePostDTO.Image.Length;
                string fileType = updatePostDTO.Image.ContentType;
                string avatarUrl = await Upload(stream, fileName, fileLength, fileType);
        
                postFound.ImageURL = avatarUrl;
            }

            postFound.Title = updatePostDTO.Title;
            postFound.Fullname = updatePostDTO.Fullname;
            postFound.BirthYear = updatePostDTO.BirthYear;
            postFound.DeathYear = updatePostDTO.DeathYear;
            postFound.Description = updatePostDTO.Description;
            postFound.Password = updatePostDTO.Password;
            db.SaveChanges();

            return Ok(new ResponseDTO(200, "Update Post success"));


        }
        //call the google api to verify the token
        private async Task<bool> VerifyToken(string token)
        {
            HttpClient httpClient = new HttpClient();

            var res =  httpClient.GetAsync($"https://www.google.com/recaptcha/api/siteverify?secret=6LepWdUhAAAAAPdhyELjzfhNCKuNZ47bSs1vb9Wn&response={token}").Result;

            if (res.StatusCode != HttpStatusCode.OK) 
            {
                return false;
            }
            string JSONres = await res.Content.ReadAsStringAsync();
            dynamic JSONdata = JObject.Parse(JSONres);

            if (JSONdata.success != "true" || JSONdata.score <= 0.5m)
            {
                return false;
            }

            return true;
        }

        protected bool CheckPostExited (int id)
        {
            var postExisted = db.Posts.Any(p => p.PostId == id);
            if(postExisted == true) return true;
            else return false;
        }
        protected Post CheckPostDeleted(int id)
        {
            var postFound = db.Posts.Where(p => p.PostId == id && p.DeletedAt == null).FirstOrDefault();
            return postFound;
            
            }
        protected async Task<string> Upload(Stream stream, string fileName, long fileLength, string fileType)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);

                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StreamContent(stream)
                    {
                        Headers =
                    {
                        ContentLength = fileLength,
                        ContentType = new MediaTypeHeaderValue(fileType)
                    }
                    }, "image", fileName);

                    var response = await client.PostAsync(this.apiUrl, content);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    dynamic dec = JsonConvert.DeserializeObject(responseBody);
                    string url = dec.data.url;
                    return url;
                }
            }
        }


    }
}
