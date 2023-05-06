using CookingCalendarApi.Enums;
using CookingCalendarApi.Extensions;
using CookingCalendarApi.Interfaces;
using CookingCalendarApi.Models;
using CookingCalendarApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CookingCalendarApi.Controllers
{
    [ApiController]
    [Route("settings")]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingsRepository _settingsRepository;
        public SettingsController(ISettingsRepository settingsRepository, IAuthRepository authRepo)
        {
            _settingsRepository = settingsRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetSettings()
        {
            return Ok(await _settingsRepository.GetUserSettings(User.GetUserId()));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSettings(UserSettingsDto settings)
        {
            settings.TrimAllStrings();
            settings.Id = User.GetUserId();

            await _settingsRepository.UpdateUserSettings(settings);
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteSettings()
        {
            await _settingsRepository.DeleteUserSettings(User.GetUserId());
            return NoContent();
        }

        [HttpGet("username/{username}/valid")]
        public async Task<IActionResult> Get([FromRoute] string username)
        {
            return Ok(await _settingsRepository.DoesUsernameExist(User.GetUserId(), username));
        }
    }
}
