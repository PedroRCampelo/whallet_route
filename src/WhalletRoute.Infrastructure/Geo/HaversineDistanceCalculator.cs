using WhalletRoute.Domain.Geo;

namespace WhalletRoute.Infrastructure.Geo;

public static class HaversineDistanceCalculator
{
    private const double EarthRadiusMeters = 6_371_000;

    public static double DistanceInMeters(Coordinate from, Coordinate to)
    {
        var lat1 = ToRadians(from.Latitude);
        var lat2 = ToRadians(to.Latitude);
        var deltaLat = ToRadians(to.Latitude - from.Latitude);
        var deltaLon = ToRadians(to.Longitude - from.Longitude);

        var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2)
                + Math.Cos(lat1) * Math.Cos(lat2) * Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadiusMeters * c;
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180;
}