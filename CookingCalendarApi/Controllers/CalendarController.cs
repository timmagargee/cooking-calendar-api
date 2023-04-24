using CookingCalendarApi.Enums;
using CookingCalendarApi.Extensions;
using CookingCalendarApi.Models;
using CookingCalendarApi.Models.DomainModels;
using CookingCalendarApi.Repositories;
using CookingCalendarApi.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace CookingCalendarApi.Controllers
{
    [ApiController]
    [Route("calendars")]
    public class CalendarController : ControllerBase
    {
        private readonly ICalendarRepository _calendarRepository;
        private readonly IRecipeRepository _recipeRepository;
        public CalendarController(ICalendarRepository calendarRepository, IRecipeRepository recipeRepository)
        {
            _calendarRepository = calendarRepository;
            _recipeRepository = recipeRepository;
        }
        //private Calendar cal = new Calendar(1, DateTime.Now, new List<Meal>()
        //    {
        //        new Meal(1, new DateOnly(2023,3,29), false, new RecipeSummary(1, "Tacos", new List<string>() { "Mexican" }, new List<string>()
        //        {
        //            "Ground Beef", "Lettuce", "Cheddar Cheese", "Taco Shells",
        //        })),
        //        new Meal(2, new DateOnly(2023, 3, 31), true, new RecipeSummary(1, "Grilled Cheese", new List<string>() { "Easy" }, new List<string>()
        //        {
        //            "White Bread", "American Cheese", "Butter",
        //        }))
        //    });

        [HttpGet("")]
        public async Task<IActionResult> GetCalendar()
        {
            return Ok(await _calendarRepository.GetCalendar(User.GetUserId()));
        }

        [HttpGet("{calendarId}/meals")]
        public async Task<IActionResult> GetCalendarMeals([FromRoute] int calendarId, [FromQuery] DateFilter filters)
        {
            if (filters.StartDate.CompareTo(filters.EndDate) > 0)
            {
                return BadRequest("Start date must come before end date");
            }

            return Ok(await _calendarRepository.GetCalendarMealDtos(calendarId, filters));
        }

        [HttpPost("{calendarId}/generate")]
        public async Task<IActionResult> GenerateCalendar([FromRoute] int calendarId, [FromBody] DateFilter filters)
        {
            filters.StartDate = filters.StartDate.Date;
            filters.EndDate = filters.EndDate.Date;
            if (filters.StartDate.CompareTo(filters.EndDate) > 0)
            {
                return BadRequest("Start date must come before end date");
            }
            //var extraInfoFilters = new DateFilter()
            //{
            //    //Extra 2 weeks to help eliminate repeat meals
            //    StartDate = filters.StartDate.Subtract(TimeSpan.FromDays(14)),
            //    EndDate = filters.EndDate
            //};
            var userId = User.GetUserId();
            var cal = await _calendarRepository.GetCalendar(userId);

            var mealAssigner = new MealAssigner(
                await _recipeRepository.GetRecipesForAssignment(userId, filters.StartDate),
                await _calendarRepository.GetCalendarMeals(calendarId, filters),
                cal.Categories
            );

            var meals = mealAssigner.AssignMealsForDates(filters);

            await _calendarRepository.AddMeals(meals.Where(x => x.Id == 0), calendarId);
            await _calendarRepository.UpdateMeals(meals.Where(x => x.Id != 0));

            return Ok();
        }

        [HttpPut("meal")]
        public async Task<IActionResult> AssignRecipe([FromBody] AssignMealDto meal)
        {
            if (meal.RecipeId == 0)
            {
                return BadRequest("RecipeId is required");
            }
            await _calendarRepository.AssignRecipeToDate(meal, User.GetUserId());
            return NoContent();
        }

        [HttpPut("{calendarId}/category")]
        public async Task<IActionResult> UpdateCategory([FromBody] Category category)
        {
            await _calendarRepository.UpdateCategory(category);
            return NoContent();
        }


    }
}
