using CookingCalendarApi.DomainModels;


namespace CookingCalendarApi.Extensions
{
    public static class RecipeExtensions
    {
        public static IEnumerable<string> GetNameList(this IEnumerable<RecipeBasicItem> items, int recipeId)
        {
            return items.Where(x => x.RecipeId == recipeId).OrderBy(x => x.SortOrder).Select(x => x.Name);
        }
    }
}
