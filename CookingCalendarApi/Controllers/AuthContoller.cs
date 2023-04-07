using CookingCalendarApi.Auth;
using CookingCalendarApi.Interfaces;
using CookingCalendarApi.Models;
using CookingCalendarApi.StartupClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CookingCalendarApi.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthContoller : ControllerBase
    {
        private IAuthRepository _repo;
        private AppConfig _appConfig;

        public AuthContoller(IAuthRepository repo, AppConfig config)
        {
            _repo = repo;
            _appConfig = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] User user)
        {
            var userId = await _repo.Login(user);
            if(userId == null || userId == -1)
            {
                return new BadRequestObjectResult("Username/Password combination does not exist");
            }
            var token = CreateToken(userId.Value);
            
            return new OkObjectResult(new LoginDto() { Token = token });
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            var userId = await _repo.CreateUser(user);
            if (userId == null || userId == -1)
            {
                return new BadRequestObjectResult("BAD");
            }
            return new OkObjectResult(CreateToken(userId.Value));
        }

        private string CreateToken(int id)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(AuthConstants.UserId, id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _appConfig.JwtToken
            ));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(claims: claims, expires: DateTime.Now.AddDays(60), signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
