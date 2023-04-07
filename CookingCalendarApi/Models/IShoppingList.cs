namespace CookingCalendarApi.Models
{
    public interface IShoppingList
    {
        DateTime CreatedOn { get; set; }
        DateOnly EndDate { get; set; }
        IEnumerable<EnteredItem> EnteredItems { get; set; }
        IEnumerable<GeneratedItem> GeneratedItems { get; set; }
        int Id { get; set; }
        DateOnly StartDate { get; set; }
    }
}