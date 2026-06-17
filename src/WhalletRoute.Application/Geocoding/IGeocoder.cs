using WhalletRoute.Domain.Geo;

namespace WhalletRoute.Application.Geocoding;

public interface IGeocoder
{
    Task<Coordinate> GeocodeAsync(string address, CancellationToken cancellationToken);
}