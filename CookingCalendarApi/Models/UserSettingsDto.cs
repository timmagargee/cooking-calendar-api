namespace CookingCalendarApi.Models
{
    public class UserSettingsDto
    {
        public UserSettingsDto(bool isDefaultMeasurementStandard, bool isDarkMode, bool isMonthDefaultView, IEnumerable<Category> categories)
        {
            this.IsDefaultMeasurementStandard = isDefaultMeasurementStandard;
            this.IsDarkMode = isDarkMode;
            this.IsMonthDefaultView = isMonthDefaultView;
            Categories = categories;
        }

        public bool IsDefaultMeasurementStandard { get; set; }
        public bool IsDarkMode { get; set; }
        public bool IsMonthDefaultView { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }
}
