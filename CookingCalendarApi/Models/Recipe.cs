using CookingCalendarApi.Enums;

namespace CookingCalendarApi.Models
{
    public class Recipe
    {
        public Recipe(int? id, string name, string description, int servingSize, IEnumerable<RecipeIngredient> ingredients, IEnumerable<RecipeTag> tags, IEnumerable<RecipeStep> steps)
        {
            Id = id;
            Name = name;
            Description = description;
            ServingSize = servingSize;
            Ingredients = ingredients;
            Tags = tags;
            Steps = steps;
        }

        public int? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ServingSize { get; set; }
        public IEnumerable<RecipeIngredient> Ingredients { get; set; }
        public IEnumerable<RecipeTag> Tags { get; set; }
        public IEnumerable<RecipeStep> Steps { get; set; }
    }

    public class RecipeIngredient
    {
        public RecipeIngredient(int? id, int ingredientId, string ingredient, MeasurementType measurement, int sortOrder, double amount, int numerator, int denominator)
        {
            Id = id;
            Ingredient = ingredient;
            Measurement = measurement;
            SortOrder = sortOrder;
            Amount = amount;
            Numerator = numerator;
            Denominator = denominator;
            IngredientId = ingredientId;
        }

        public int? Id { get; set; }
        public int IngredientId { get; set; }
        public string Ingredient { get; set; }
        public MeasurementType Measurement { get; set; }
        public int SortOrder { get; set; }
        public double Amount { get; set; }
        public int Numerator { get; set; }
        public int Denominator { get; set; }
    }

    public class RecipeTag
    {
        public RecipeTag(int? id, int? tagId, string name)
        {
            Id = id;
            TagId = tagId;
            Name = name;
        }

        public int? Id { get; set; }
        public int? TagId { get; set; }
        public string Name { get; set; }
    }

    public class RecipeStep
    {
        public RecipeStep(int? id, string step, int sortOrder, IEnumerable<RecipeStepIngredient> ingredients)
        {
            Id = id;
            Step = step;
            SortOrder = sortOrder;
            Ingredients = ingredients;
        }

        public int? Id { get; set; }
        public string Step { get; set; }
        public int SortOrder { get; set; }
        public IEnumerable<RecipeStepIngredient> Ingredients { get; set; }
    }

    public class RecipeStepIngredient
    {
        public RecipeStepIngredient(int? id, int? recipeIngredientId, int? ingredientId, double numerator, int denominator)
        {
            Id = id;
            RecipeIngredientId = recipeIngredientId;
            IngredientId = ingredientId;
            Numerator = numerator;
            Denominator = denominator;
        }

        public int? Id { get; set;}
        public int? RecipeIngredientId { get; set; }
        public int? IngredientId { get; set; }
        public double Numerator { get; set; }
        public int Denominator { get; set; }
    }

}