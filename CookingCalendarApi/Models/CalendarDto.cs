namespace CookingCalendarApi.Models
{
    public class CalendarDto
    {
        public int Id { get; set; }
        public DateTime LastGenerated { get; set; }
        public IEnumerable<Category>  Categories { get; set; }
        //public IEnumerable<Meal> Meals { get; set; }
    }

    public class BaseMeal
    {
        public int Id { get; set; }
        public DateTime MealDate { get; set; }
        public bool IsUserAssigned { get; set; }
    }

    public class Meal : BaseMeal
    {
        public int RecipeId { get; set; }
    }

    public class MealDto : BaseMeal
    {
        public RecipeSummary Recipe { get; set; }
    }

    public class DateFilter
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class AssignMealDto
    {
        public int? MealId { get; set; }
        public DateTime MealDate { get; set; }
        public int RecipeId { get; set; }
    }
}
