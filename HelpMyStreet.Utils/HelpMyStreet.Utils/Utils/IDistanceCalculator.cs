namespace UserService.Core.Utils
{
    public interface IDistanceCalculator
    {
        double GetDistanceInMetres(double latitude, double longitude, double otherLatitude, double otherLongitude);

        double GetDistanceInMiles(double latitude, double longitude, double otherLatitude, double otherLongitude);
    }
}