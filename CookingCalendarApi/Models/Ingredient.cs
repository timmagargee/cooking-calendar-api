namespace CookingCalendarApi.Models
{
    public class Ingredient
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public bool IsMeat { get; set; }
        public bool IsDairy { get; set; }
        public bool IsGluten { get; set; }
    }

    public class IngredientBase
    {
        public int? UserIngredientId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
