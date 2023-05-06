namespace CookingCalendarApi.Models
{
    public class UserSettingsDto
    {
        public int? Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public bool IsDefaultMeasurementStandard { get; set; }
        public bool IsDarkMode { get; set; }
        public bool IsMonthDefaultView { get; set; }
        public int? DefaultShoppingDay { get; set; }
        public int? DefaultServings { get; set; }
    }
}
