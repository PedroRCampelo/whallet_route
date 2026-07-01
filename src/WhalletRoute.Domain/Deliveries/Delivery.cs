using WhalletRoute.Domain.Geo;

namespace WhalletRoute.Domain.Deliveries;

public sealed class Delivery
{
    private readonly List<StatusChange> _history = new();

    public Guid Id { get; }
    public string ExternalId { get; }
    public string ClientId { get; }
    public string ClientName { get; }
    public string Address { get; }
    public Coordinate? Coordinate { get; private set; }
    public double WeightKg { get; }
    public double VolumeM3 { get; }
    public DeliveryWindow? Window { get; }
    public string? Phone { get; }
    public string? Instructions { get; }
    public int? Order { get; private set; }
    public DeliveryStatus Status { get; private set; }
    public IReadOnlyList<StatusChange> History => _history;

    public Delivery(
        string externalId, string clientId, string clientName, string address,
        double weightKg = 0, double volumeM3 = 0, DeliveryWindow? window = null,
        string? phone = null, string? instructions = null)
    {
        if (string.IsNullOrWhiteSpace(externalId))
            throw new ArgumentException("External id is required.", nameof(externalId));
        if (string.IsNullOrWhiteSpace(clientId))
            throw new ArgumentException("Client id is required.", nameof(clientId));

        Id = Guid.NewGuid();
        ExternalId = externalId;
        ClientId = clientId;
        ClientName = clientName;
        Address = address;
        WeightKg = weightKg;
        VolumeM3 = volumeM3;
        Window = window;
        Phone = phone;
        Instructions = instructions;
        Status = DeliveryStatus.Pending;
    }

    public void SetCoordinate(Coordinate coordinate) => Coordinate = coordinate;

    public void SetOrder(int order)
    {
        if (order < 1)
            throw new ArgumentOutOfRangeException(nameof(order), "Order must be positive.");

        Order = order;
    }

    public void MarkEnRoute()
        => Transition(DeliveryStatus.EnRoute, DeliveryStatus.Pending);

    public void MarkDelivered(Coordinate? location = null, string? note = null)
        => Transition(DeliveryStatus.Delivered, DeliveryStatus.EnRoute, location, note);

    public void MarkRefused(string reason, Coordinate? location = null)
        => Transition(DeliveryStatus.Refused, DeliveryStatus.EnRoute, location, reason);

    private void Transition(DeliveryStatus to, DeliveryStatus requiredFrom, Coordinate? location = null, string? note = null)
    {
        if (Status != requiredFrom)
            throw new InvalidOperationException($"Cannot change delivery from {Status} to {to}.");

        Status = to;
        _history.Add(new StatusChange { To = to, OccurredAt = DateTime.UtcNow, Location = location, Note = note });
    }
}