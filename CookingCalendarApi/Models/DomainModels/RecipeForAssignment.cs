namespace CookingCalendarApi.Models.DomainModels
{
    public class RecipeForAssignment
    {
        public int Id { get; set; }
        public IEnumerable<int> TagIds { get; set; }
        public IEnumerable<int> IngredientIds { get; set; }
        public DateTime? LastMade { get; set; }
        public int Servings { get; set; }
    }
}
