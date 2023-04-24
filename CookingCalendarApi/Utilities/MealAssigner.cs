using CookingCalendarApi.Enums;
using CookingCalendarApi.Models;
using CookingCalendarApi.Models.DomainModels;

namespace CookingCalendarApi.Utilities
{
    public class MealAssigner
    {

        public List<RecipeForAssignment> Recipes { get; set; }
        public List<Meal> Meals { get; set; }
        public IEnumerable<Category> Categories { get; set; }

        public MealAssigner(IEnumerable<RecipeForAssignment> recipes, IEnumerable<Meal> meals, IEnumerable<Category> categories)
        {
            Recipes = recipes.ToList();
            Meals = meals.ToList();
            Categories = categories;
        }      

        public IEnumerable<Meal> AssignMealsForDates(DateFilter filters)
        {
            var userAssignedDates = Meals.Where(x => x.IsUserAssigned).Select(x => x.MealDate);
            var unplannedDays = Categories.Where(x => x.CategoryType == CategoryType.Unplanned).Select(x => x.DayOfWeek);

            var datesToAssign = Enumerable.Range(0, 1 + filters.EndDate.Date.Subtract(filters.StartDate.Date).Days)
                .Select(offset => filters.StartDate.AddDays(offset))
                .Where(x => !(unplannedDays.Any(y => y == x.DayOfWeek) || userAssignedDates.Any(y => y.Date == x.Date)))
                .ToList();


            var mealsToAdd = new List<Meal>();

            //Assign ingredients
            foreach (var cat in Categories.Where(x => x.CategoryType == CategoryType.Ingredient))
            {
                mealsToAdd.AddRange(GetMealsToAdd(
                    Recipes.Where(x => x.IngredientIds.Contains(cat.IngredientId.GetValueOrDefault())).ToList(),
                    datesToAssign.Where(x => x.DayOfWeek == cat.DayOfWeek))
                );
            }

            //Assign tags
            foreach (var cat in Categories.Where(x => x.CategoryType == CategoryType.Tag))
            {
                mealsToAdd.AddRange(GetMealsToAdd(
                    Recipes.Where(x => x.TagIds.Contains(cat.TagId.GetValueOrDefault())).ToList(),
                    datesToAssign.Where(x => x.DayOfWeek == cat.DayOfWeek))
                );
            }

            //Assign randoms TODO change if more category types added
            var randomCategoryDays = Categories
                .Where(x => x.CategoryType == CategoryType.Random)
                .Select(x => x.DayOfWeek);
            mealsToAdd.AddRange(GetMealsToAdd(
                Recipes,
                datesToAssign.Where(x => randomCategoryDays.Contains(x.DayOfWeek)))
            );

            return mealsToAdd;
        }
        private IEnumerable<Meal> GetMealsToAdd(List<RecipeForAssignment> recipes, IEnumerable<DateTime> dates)
        {
            var meals = new List<Meal>();
            if (recipes.Any())
            {
                foreach (var date in dates)
                {
                    //TODO make the maximum difference between other occurence to account for recipes assigned after this point
                    var recipe = recipes.MinBy(x => x.LastMade.GetValueOrDefault().Ticks)!;
                    recipe.LastMade = date;
                    var meal = Meals.FirstOrDefault(x => x.MealDate.Date == date);
                    if (meal != null)
                    {
                        meal.MealDate = date;
                        meal.RecipeId = recipe.Id;
                    }
                    else
                    {
                        meal = new Meal
                        {
                            RecipeId = recipe.Id,
                            MealDate = date,
                            IsUserAssigned = false
                        };
                    }
                    meals.Add(meal);
                }
                UpdateRecipeDates(meals);
            }
            return meals;
        }
        private void UpdateRecipeDates(IEnumerable<Meal> newMeals)
        {
            foreach (var meal in newMeals)
            {
                var recipe = Recipes.First(x => x.Id == meal.RecipeId);
                if (recipe.LastMade == null || recipe.LastMade.Value < meal.MealDate)
                {
                    recipe.LastMade = meal.MealDate;
                }
            }
        }
    }
}
