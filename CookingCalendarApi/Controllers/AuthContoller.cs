using CookingCalendarApi.Auth;
using CookingCalendarApi.Extensions;
using CookingCalendarApi.Interfaces;
using CookingCalendarApi.Models;
using CookingCalendarApi.Repositories;
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
        private readonly ICalendarRepository _calendarRepository;
        private AppConfig _appConfig;

        public AuthContoller(IAuthRepository repo, AppConfig config, ICalendarRepository calendarRepository)
        {
            _repo = repo;
            _calendarRepository = calendarRepository;
            _appConfig = config;
        }

        [HttpGet("usernames")]
        public async Task<IActionResult> GetUsernamesInUse()
        {
            return new OkObjectResult(await _repo.GetUsernamesInUse());
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] User user)
        {
            var userId = await _repo.Login(user);
            if(userId == -1)
            {
                return new BadRequestObjectResult("Username/Password combination does not exist");
            }
            var token = CreateToken(userId);
            
            return new OkObjectResult(new LoginDto() { Token = token });
        }


        [HttpPut("update/password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            dto.UserId = User.GetUserId();
            dto.TrimAllStrings();
            var response = await _repo.ChangePassword(dto);
            switch (response)
            {
                case 1:
                    return new OkResult();
                case -1:
                    return new BadRequestObjectResult("Current password input was not correct");
                default:
                    return new BadRequestObjectResult("Password could not be verified");
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            user.TrimAllStrings();
            var userNames = await _repo.GetUsernamesInUse();
            if(userNames.Contains(user.Username))
            {
                return new BadRequestObjectResult("Username Already Exists");
            }
            var userId = await _repo.CreateUser(user);
            if (userId == null || userId == -1)
            {
                return new BadRequestObjectResult("Account could not be created");
            }

            await _calendarRepository.CreateNewCalendar(userId.Value);

            return new OkObjectResult(new LoginDto { Token = CreateToken(userId.Value) });
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
