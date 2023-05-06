using CookingCalendarApi.Models.DomainModels;
using CookingCalendarApi.Models;
using CookingCalendarApi.DomainModels;
using CookingCalendarApi.Enums;

namespace CookingCalendarApi.Utilities
{
    public class ShoppingListGenerator
    {
        public ShoppingListGenerator(IEnumerable<ShoppingIngredient> ingredients)
        {
            Ingredients = ingredients.ToList();
        }

        public List<ShoppingIngredient> Ingredients { get; set; }

        public IEnumerable<GeneratedItem> CreateShoppingList(int? servings)
        {
            var items = new List<GeneratedItem>();

            foreach (var ing in Ingredients.GroupBy(x => x.IngredientId))
            {
                RecipeAmount? total = null;
                MeasurementType meas = MeasurementType.Amount;
                foreach(var amount in ing.Select(x => x.Amount))
                {
                    if(servings.HasValue)
                    {
                        amount.UpdateServings(servings.Value); 
                    }
                    if(total == null)
                    {
                        meas = MeasurementConverter.GetDefaultMeasurement(amount.Measurement);
                        total = amount.ConvertTo(meas);
                    }
                    else
                    {
                        try
                        {
                            amount.ConvertTo(meas);
                            total.Add(amount);
                        }
                        catch (Exception ex)
                        {
                            //TODO Handle Ingredients that have multiple different types of measurements
                            Console.WriteLine(ex.ToString());
                        }
                    }
                }
                items.Add(new GeneratedItem
                {
                    IngredientId = ing.Key,
                    IsChecked = false,
                    Amount = total!.GetSingleAmount(),
                    MeasurementType = total.Measurement,
                });
            }

            return items;
        }
    }
}
