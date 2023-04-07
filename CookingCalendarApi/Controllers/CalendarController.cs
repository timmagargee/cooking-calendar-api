using CookingCalendarApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace CookingCalendarApi.Controllers
{
    [ApiController]
    [Route("calendars")]
    public class CalendarController : ControllerBase
    {
        private Calendar cal = new Calendar(1, DateTime.Now, new List<Meal>()
            {
                new Meal(1, new DateOnly(2023,3,29), false, new RecipeReferenceInfo(1, "Tacos", new List<string>() { "Mexican" }, new List<string>()
                {
                    "Ground Beef", "Lettuce", "Cheddar Cheese", "Taco Shells",
                })),
                new Meal(2, new DateOnly(2023, 3, 31), true, new RecipeReferenceInfo(1, "Grilled Cheese", new List<string>() { "Easy" }, new List<string>()
                {
                    "White Bread", "American Cheese", "Butter",
                }))
            });

        [HttpGet("{calendarId}", Name="GetCalendar")]
        public IActionResult GetCalendar([FromRoute] int calendarId, [FromQuery] DateFilter filters)
        {
            if(filters.StartDate.CompareTo(filters.EndDate) > 0)
            {
                return BadRequest("Start date must come before end date");
            }
            return Ok(cal);
        }

        [HttpPost("{calendarId}")]
        public IActionResult GenerateCalendar([FromRoute] int calendarId, [FromQuery] DateFilter filters)
        {
            if (filters.StartDate.CompareTo(filters.EndDate) > 0)
            {
                return BadRequest("Start date must come before end date");
            }
            return Created(nameof(GetCalendar), cal);
        }

        [HttpPut("{calendarId}/meal")]
        public IActionResult AssignRecipe([FromBody] AssignMeal meal)
        {
            if(meal.RecipeId == 0)
            {
                return BadRequest("RecipeId is required");
            }
            if(meal.MealId == null)
            {
                return Created(nameof(GetCalendar), null);
            }
            return NoContent();
        }

        [HttpPut("{calendarId}/category")]
        public IActionResult UpdateCategory([FromBody] Category category)
        {
            if (category.CategoryType == 0)
            {
                return BadRequest("CategoryType is required");
            }
            if (category.Id == null)
            {
                return Created(nameof(GetCalendar), null);
            }
            return NoContent();
        }
    }
}
