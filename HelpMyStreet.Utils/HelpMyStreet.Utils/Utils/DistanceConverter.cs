using System;

namespace HelpMyStreet.Utils.Utils
{
    public class DistanceConverter
    {
        public static int MilesToMetres(double miles)
        {
            return (int)Math.Round(miles * 1609.344d, 0);
        }

        public static double MetresToMiles(int metres)
        {
            return metres / 1609.344d;
        }
        public static double MetresToMiles(double metres)
        {
            return metres / 1609.344d;
        }
    }
}
