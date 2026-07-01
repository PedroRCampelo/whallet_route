using System.Collections.Concurrent;
using WhalletRoute.Application.Geocoding;
using WhalletRoute.Domain.Geo;

namespace WhalletRoute.Infrastructure.Geocoding;

public sealed class InMemoryGeocodeCache : IGeocodeCache
{
    private readonly ConcurrentDictionary<string, Coordinate> _entries = new();

    public bool TryGet(string address, out Coordinate? coordinate)
    {
        return _entries.TryGetValue(Normalize(address), out coordinate);
    }

    public void Set(string address, Coordinate coordinate)
    {
        _entries[Normalize(address)] = coordinate;
    }

    private static string Normalize(string address)
    {
        return address.Trim().ToLowerInvariant();
    }
}