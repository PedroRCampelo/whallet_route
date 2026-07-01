namespace WhalletRoute.Domain.Deliveries;

public sealed record DeliveryWindow
{
    public TimeOnly From { get; }
    public TimeOnly To { get; }

    public DeliveryWindow(TimeOnly from, TimeOnly to)
    {
        if (to <= from)
            throw new ArgumentException("Delivery window end must be after its start.");

        From = from;
        To = to;
    }
}