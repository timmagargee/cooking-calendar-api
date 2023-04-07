namespace CookingCalendarApi.Models
{
    public class UserSettings
    {
        public UserSettings(bool isDefaultMeasurementStandard, bool isDarkMode, bool isMonthDefaultView, IEnumerable<Category> categories)
        {
            this.isDefaultMeasurementStandard = isDefaultMeasurementStandard;
            this.isDarkMode = isDarkMode;
            this.isMonthDefaultView = isMonthDefaultView;
            Categories = categories;
        }

        public bool isDefaultMeasurementStandard { get; set; }
        public bool isDarkMode { get; set; }
        public bool isMonthDefaultView { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }
}
