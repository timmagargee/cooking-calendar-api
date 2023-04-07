using CookingCalendarApi.Enums;

namespace CookingCalendarApi.Models
{
    public class ShoppingList
    {
        public ShoppingList(int id, DateOnly startDate, DateOnly endDate, DateTime createdOn, IEnumerable<GeneratedItem> generatedItems, IEnumerable<EnteredItem> enteredItems)
        {
            Id = id;
            StartDate = startDate;
            EndDate = endDate;
            CreatedOn = createdOn;
            GeneratedItems = generatedItems;
            EnteredItems = enteredItems;
        }

        public int Id { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public IEnumerable<GeneratedItem> GeneratedItems { get; set; }
        public IEnumerable<EnteredItem> EnteredItems { get; set; }
    }

    public class ShoppingListUpdate
    {
        public ShoppingListUpdate(int id, IEnumerable<int>? generatedItemIdsToCheck, IEnumerable<int>? generatedItemIdsToUnCheck, IEnumerable<int>? generatedItemIdsToDelete, IEnumerable<EnteredItem>? enteredItemsToUpdate, IEnumerable<int>? enteredItemIdsToDelete)
        {
            Id = id;
            GeneratedItemIdsToCheck = generatedItemIdsToCheck;
            GeneratedItemIdsToUnCheck = generatedItemIdsToUnCheck;
            GeneratedItemIdsToDelete = generatedItemIdsToDelete;
            EnteredItemsToUpdate = enteredItemsToUpdate;
            EnteredItemIdsToDelete = enteredItemIdsToDelete;
        }

        public int Id { get; set; }
        public IEnumerable<int>? GeneratedItemIdsToCheck { get; set; }
        public IEnumerable<int>? GeneratedItemIdsToUnCheck { get; set; }
        public IEnumerable<int>? GeneratedItemIdsToDelete { get; set; }
        public IEnumerable<EnteredItem>? EnteredItemsToUpdate { get; set; }
        public IEnumerable<int>? EnteredItemIdsToDelete { get; set; }
    }


    public class GeneratedItem
    {
        public GeneratedItem(int id, bool isChecked, int ingredientId, string name, double amount, MeasurementType measurementType)
        {
            Id = id;
            IsChecked = isChecked;
            IngredientId = ingredientId;
            Name = name;
            Amount = amount;
            MeasurementType = measurementType;
        }

        public int Id { get; set; }
        public bool IsChecked { get; set; }
        public int IngredientId { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
        public MeasurementType MeasurementType { get; set; }

    }

    public class EnteredItem
    {
        public EnteredItem(int id, bool isChecked, string name, ShoppingCategory category)
        {
            Id = id;
            IsChecked = isChecked;
            Name = name;
            Category = category;
        }

        public int? Id { get; set; }
        public bool IsChecked { get; set; }
        public string Name { get; set; }
        public ShoppingCategory Category { get; set; }
    }
}
