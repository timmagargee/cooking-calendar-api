namespace CookingCalendarApi.Enums
{
    public enum MeasurementType
    {
        Amount = 0,
        #region Standard
        Tsp = 1,
        Tbl = 2,
        FlOz = 3,
        //Gill = 4, //??
        Cup = 5,
        Pt = 6,
        Qt = 7,
        Gal = 8,
        Lb = 9,
        Oz = 10,
        In = 11,
        Yd = 12,
        F = 13,
        #endregion

        #region Metric
        ML = 101,
        L = 102,
        DL = 103,

        G = 104,
        Kg = 105,

        MM = 106,
        CM = 107,
        M = 108,
        Celcius = 109,
        #endregion
    }
}
