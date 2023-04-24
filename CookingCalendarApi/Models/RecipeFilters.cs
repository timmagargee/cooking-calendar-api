namespace CookingCalendarApi.Models
{
    public class RecipeFilters
    {
        public int? Limit { get; set; }
        public int? Offset { get; set; }
        public string? Name { get; set; }
        public IEnumerable<int>? TagIds { get; set; }
        public bool? MustBeVegitarian { get; set; }
        public bool? MustBeDairyFree { get; set; }
        public bool? MustBeGlutenFree { get; set; }
        public IEnumerable<int>? IngredientsToInclude { get; set; }
        public IEnumerable<int>? IngredientsToExclude { get; set; }
        public DateTime? NotMadeSinceDate { get; set; }
    }

    public class RecipeSummary
    {
        public RecipeSummary() { }
        public RecipeSummary(int id, string name, IEnumerable<string> tags, IEnumerable<string> ingredients)
        {
            Id = id;
            Name = name;
            Tags = tags;
            Ingredients = ingredients;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public IEnumerable<string> Ingredients { get; set; }
    }

}
