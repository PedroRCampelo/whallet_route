using WhalletRoute.Domain.Geo;

namespace WhalletRoute.Domain.Deliveries;

public sealed class Delivery
{
    private readonly List<StatusChange> _history = new();

    public Guid Id { get; private set; }
    public string ExternalId { get; private set; } = null!;
    public string ClientId { get; private set; } = null!;
    public string ClientName { get; private set; } = null!;
    public string Address { get; private set; } = null!;
    public Coordinate? Coordinate { get; private set; }
    public double WeightKg { get; private set; }
    public double VolumeM3 { get; private set; }
    public DeliveryWindow? Window { get; private set; }
    public string? Phone { get; private set; }
    public string? Instructions { get; private set; }
    public int? Order { get; private set; }
    public DeliveryStatus Status { get; private set; }
    public IReadOnlyList<StatusChange> History => _history;

    private Delivery() { }
    
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