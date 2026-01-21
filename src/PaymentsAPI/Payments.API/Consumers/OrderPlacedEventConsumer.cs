using Contracts.Events;
using MassTransit;

namespace Payments.API.Consumers;

public sealed class OrderPlacedEventConsumer : IConsumer<OrderPlacedEvent>
{
    private readonly ILogger<OrderPlacedEventConsumer> _logger;

    public OrderPlacedEventConsumer(ILogger<OrderPlacedEventConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderPlacedEvent> context)
    {
        var msg = context.Message;

        _logger.LogInformation(
            "Processing payment for OrderId={OrderId} UserId={UserId} GameId={GameId} Price={Price}",
            msg.OrderId,
            msg.UserId,
            msg.GameId,
            msg.Price);

        // Deterministic simulation: ~80% Approved (based on first byte of OrderId)
        var firstByte = msg.OrderId.ToByteArray()[0];
        var status = (firstByte % 5 == 0) ? PaymentStatus.Rejected : PaymentStatus.Approved;

        _logger.LogInformation("Payment result for OrderId={OrderId}: {Status}", msg.OrderId, status);

        await context.Publish(new PaymentProcessedEvent(
            msg.OrderId,
            msg.UserId,
            msg.GameId,
            msg.Price,
            status));
    }
}