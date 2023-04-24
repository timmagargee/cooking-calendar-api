using CookingCalendarApi.Enums;
using CookingCalendarApi.Models;
using CookingCalendarApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CookingCalendarApi.Controllers
{
    [ApiController]
    [Route("settings")]
    public class SettingsController : ControllerBase
    {
        private readonly IRecipeRepository _settingsRepository;
        public SettingsController(IRecipeRepository settingsRepository)
        {
            _settingsRepository = settingsRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetSettings()
        {
            //return BadRequest("User does not exist");

            return Ok(new UserSettingsDto(true, true, false,
                new List<Category>()
                {
                    //new Category(1, "Mystery", DayOfWeek.Monday, CategoryType.Random, null, null),
                    //new Category(1, "Taco Tuesday", DayOfWeek.Tuesday, CategoryType.Tag, 1, null),
                    //new Category(1, "Chicken Day", DayOfWeek.Thursday, CategoryType.Ingredient, null, 1),
                    //new Category(1, "Take Out", DayOfWeek.Sunday, CategoryType.Unplanned, null, null)
                }
            ));
        }

        [HttpPut]
        public IActionResult UpdateSettings(UserSettingsDto settings)
        {
            //return BadRequest("User does not exist");

            //        return Ok(new UserSettings(false, false, true,
            //new List<Category>()
            //{
            //                new Category(null, "Mystery", DayOfWeek.Monday, CategoryType.Random, null, null),
            //                new Category(null, "Taco Tuesday", DayOfWeek.Tuesday, CategoryType.Tag, 1, null),
            //                new Category(null, "Chicken Day", DayOfWeek.Thursday, CategoryType.Ingredient, null, 1),
            //                new Category(null, "Take Out", DayOfWeek.Sunday, CategoryType.Unplanned, null, null)
            //}
            //));
            return NoContent();
        }
    }
}
