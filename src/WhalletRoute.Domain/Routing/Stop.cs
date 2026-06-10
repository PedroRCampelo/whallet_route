using WhalletRoute.Domain.Geo;

namespace WhalletRoute.Domain.Routing;

public sealed class Stop
{
    public string Id { get; }
    public Coordinate Coordinate { get; }

    public Stop(string id, Coordinate coordinate)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Stop id is required.", nameof(id));

        Id = id;
        Coordinate = coordinate;
    }
}