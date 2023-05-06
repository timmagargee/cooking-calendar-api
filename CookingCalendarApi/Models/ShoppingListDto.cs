using CookingCalendarApi.Enums;

namespace CookingCalendarApi.Models
{
    public class ShoppingListDto
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public IEnumerable<GeneratedItem> GeneratedItems { get; set; }
        public IEnumerable<EnteredItem> EnteredItems { get; set; }
    }

    public class ShoppingListUpdate
    {
        public int Id { get; set; }
        public IEnumerable<int>? GeneratedItemIdsToCheck { get; set; }
        public IEnumerable<int>? GeneratedItemIdsToUnCheck { get; set; }
        public IEnumerable<int>? GeneratedItemIdsToDelete { get; set; }
        public IEnumerable<EnteredItem>? EnteredItemsToUpdate { get; set; }
        public IEnumerable<int>? EnteredItemIdsToDelete { get; set; }
    }

    public class ShoppingItem
    {
        public int? Id { get; set; }
        public bool IsChecked { get; set; }
        public string Name { get; set; }
    }

    public class GeneratedItem : ShoppingItem
    {
        public int IngredientId { get; set; }
        public double Amount { get; set; }
        public MeasurementType MeasurementType { get; set; }

    }

    public class EnteredItem : ShoppingItem
    {
        public ShoppingCategory Category { get; set; }
    }
}
