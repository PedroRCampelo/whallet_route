using WhalletRoute.Domain.Geo;

namespace WhalletRoute.Application.Geocoding;

public interface IGeocodeCache
{
    bool TryGet(string address, out Coordinate? coordinate);
    void Set(string address, Coordinate coordinate);
}