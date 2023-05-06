using CookingCalendarApi.Enums;
using Microsoft.Data.SqlClient;

namespace CookingCalendarApi.Models
{
    public class Recipe
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Servings { get; set; }
        public bool AreMeasurementsStandard { get; set; }
        public IEnumerable<RecipeIngredient> Ingredients { get; set; }
        public IEnumerable<RecipeTag> Tags { get; set; }
        public IEnumerable<RecipeStep> Steps { get; set; }
    }

    public class RecipeIngredient
    {
        public int IngredientId { get; set; }
        public string Ingredient { get; set; }
        public MeasurementType Measurement { get; set; }
        public int SortOrder { get; set; }
        public double Amount { get; set; }
        public int AmountNumerator { get; set; }
        public int AmountDenominator { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class RecipeTag
    {
        public int? TagId { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
    }

    public class RecipeStep
    {
        public string Step { get; set; }
        public int SortOrder { get; set; }
    }
}