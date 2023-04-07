namespace CookingCalendarApi.Models
{
    public class Calendar
    {
        public Calendar(int id, DateTime lastGenerated, IEnumerable<Meal> meals)
        {
            Id = id;
            LastGenerated = lastGenerated;
            Meals = meals;
        }

        public int Id { get; set; }
        public DateTime LastGenerated { get; set; }
        public IEnumerable<Meal> Meals { get; set; }
    }

    public class Meal
    {
        public Meal(int id, DateOnly mealDate, bool isUserAssigned, RecipeReferenceInfo recipe)
        {
            Id = id;
            MealDate = mealDate;
            IsUserAssigned = isUserAssigned;
            Recipe = recipe;
        }

        public int Id { get; set; }
        public DateOnly MealDate { get; set; }
        public bool IsUserAssigned { get; set; }
        public RecipeReferenceInfo Recipe { get; set; }
    }

    public class DateFilter
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class AssignMeal
    {
        public int? MealId { get; set; }
        public DateOnly MealDate { get; set; }
        public int RecipeId { get; set; }
    }
}
