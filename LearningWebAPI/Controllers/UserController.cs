using LearningWebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LearningWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly MovieContext _dbContext;
        private readonly IConfiguration _config;
        public UserController(MovieContext dbContext, IConfiguration config)
        {
            _dbContext = dbContext;
            _config = config;
        }

        // To generate token
        private string GenerateToken(UserModel user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            Debug.WriteLine("jwt key : " +_config["Jwt:Key"]);
            Debug.WriteLine("jwt Issuer : " + _config["Jwt:Issuer"]);
            Debug.WriteLine("jwt Audience : " + _config["Jwt:Audience"]);
            Debug.WriteLine("data user " + user.Username.ToString());
            Debug.WriteLine("data user " + user.Role.ToString());
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.Username),
                new Claim(ClaimTypes.Role,user.Role)
            };
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);


            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        [HttpPost]
        public async Task<ActionResult<UserModel>> Login(UserModel userModel)
        {
            if (_dbContext == null)
            {
                return NotFound();
            }

            var user = await _dbContext.Users.Where(m => m.Username == userModel.Username && m.Password == userModel.Password)
    .FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                var token = GenerateToken(user);
                //return Ok(token);
                return Ok(new { response = token });
            }
            return user;
        }

    }
}
