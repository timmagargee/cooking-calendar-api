namespace CookingCalendarApi.Utilities
{
    public static class FractionUtility
    {
        public static int GCD(int n, int d)
        {
            while (d != 0)
            {
                int t = d;
                d = n % d;
                n = t;
            }
            return n;
        }
    }
}
