using Life_API.DTO;
using Life_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Life_API.Controllers
{
    
    [ApiController]
    [Route("api/auth")]
    public class AccountController : ControllerBase
    {
        public readonly AppDBContext db;
        private IConfiguration configuration;

        public AccountController(AppDBContext db, IConfiguration configuration)
        {
            this.db = db;
            this.configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginUserDTO loginUser)
        {
            var f_password = GetMD5(loginUser.Password);

            var userFound = db.Users.FirstOrDefault(u => u.UserName == loginUser.Username && u.Password == f_password);
            
            if(userFound != null)
            {
                var token = Generate(userFound);
                var userResponse = new LoginResponseDTO()
                {
                    UserId = userFound.UserId,
                    Email = userFound.Email,
                    Fullname = userFound.Fullname,
                    AccessToken = token,
                    Username = userFound.UserName,
                    Role = userFound.Role
                };
                    return Ok(userResponse);
            }
            return NotFound("User not found");

        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterUserDTO registerUserDTO)
        {
            
                var checkEmailDup = db.Users.FirstOrDefault(s => s.Email == registerUserDTO.Email);

                if (checkEmailDup == null)
                {
                    registerUserDTO.Password = GetMD5(registerUserDTO.Password);
                    var user = new User()
                    {
                        UserName = registerUserDTO.Username,
                        Password = registerUserDTO.Password,
                        Email = registerUserDTO.Email,
                        Phone = registerUserDTO.Phone,
                        Fullname = registerUserDTO.Fullname,
                        Role = "User"
                    };
                    db.Users.Add(user);
                    db.SaveChanges();
                    return Ok();
                }
                else return NotFound("Email is duplicated!!");
        }

        private string Generate(User userFound)
        {
            var secutiryKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(secutiryKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFound.UserName),
                new Claim(ClaimTypes.Email, userFound.Email),
                new Claim(ClaimTypes.GivenName, userFound.Fullname),
                new Claim(ClaimTypes.Surname, userFound.Fullname),
                new Claim(ClaimTypes.Role, userFound.Role),
            };

            var token = new JwtSecurityToken(configuration["Jwt:Issuer"], configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        //create a string MD5
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
        }
    }
}
