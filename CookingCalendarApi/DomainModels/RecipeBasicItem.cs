namespace CookingCalendarApi.DomainModels
{
    public class RecipeBasicItem
    {
        public int RecipeId { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
    }

    public class RecipeBasicItemId
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
    }
}
