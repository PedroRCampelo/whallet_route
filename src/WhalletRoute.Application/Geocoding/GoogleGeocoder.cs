using System.Net.Http.Json;
using WhalletRoute.Application.Geocoding;
using WhalletRoute.Domain.Geo;

namespace WhalletRoute.Infrastructure.Geocoding;

public sealed class GoogleGeocoder : IGeocoder
{
    private readonly HttpClient _httpClient;
    private readonly IGeocodeCache _cache;
    private readonly string _apiKey;

    public GoogleGeocoder(HttpClient httpClient, IGeocodeCache cache, string apiKey)
    {
        _httpClient = httpClient;
        _cache = cache;
        _apiKey = apiKey;
    }

    public async Task<Coordinate> GeocodeAsync(string address, CancellationToken cancellationToken)
    {
        // Try to get the address in Cache
        if (_cache.TryGet(address, out var cached) && cached is not null)
            return cached;

        var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(address)}&key={_apiKey}";

        var response = await _httpClient.GetFromJsonAsync<GoogleGeocodeResponse>(url, cancellationToken)
                       ?? throw new InvalidOperationException("Empty response from geocoding service.");

        if (response.Status != "OK" || response.Results.Count == 0)
            throw new InvalidOperationException($"Could not geocode address '{address}'. Status: {response.Status}.");

        var location = response.Results[0].Geometry.Location;
        var coordinate = new Coordinate(location.Lat, location.Lng);

        _cache.Set(address, coordinate);

        return coordinate;
    }

    private sealed record GoogleGeocodeResponse
    {
        public required string Status { get; init; }
        public required IReadOnlyList<Result> Results { get; init; }
    }

    private sealed record Result
    {
        public required Geometry Geometry { get; init; }
    }

    private sealed record Geometry
    {
        public required Location Location { get; init; }
    }

    private sealed record Location
    {
        public double Lat { get; init; }
        public double Lng { get; init; }
    }
}