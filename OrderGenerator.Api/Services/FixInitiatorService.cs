using OrderGenerator.Api.Dtos;
using OrderGenerator.Api.Services.Interfaces;
using QuickFix;
using QuickFix.Fields;
using QuickFix.FIX44;
using QuickFix.Logger;
using QuickFix.Store;
using QuickFix.Transport;

namespace OrderGenerator.Api.Services;

public class FixInitiatorService : IFixInitiatorService
{
    private readonly string _configFile;
    private IInitiator? _initiator;
    private readonly OrderGeneratorApplication _application;

    public FixInitiatorService(string configFile, OrderGeneratorApplication application)
    {
        _configFile = configFile ?? throw new ArgumentNullException(nameof(configFile));
        _application = application ?? throw new ArgumentNullException(nameof(application));
    }

    public void Start()
    {
        var settings = new SessionSettings(_configFile);
        var storeFactory = new FileStoreFactory(settings);
        var logFactory = new FileLogFactory(settings);

        _initiator = new SocketInitiator(_application, storeFactory, settings, logFactory);
        _initiator.Start();

        Console.WriteLine("Initiator started");
    }

    public void Stop()
    {
        _initiator?.Stop();
        Console.WriteLine("Initiator stopped.");
    }

    public OrderResponse SendOrder(OrderRequest order)
    {
        if (_application?.SessionID == null)
            return new OrderResponse("Something went wrong!", Guid.Empty, "Error");

        var orderId = Guid.NewGuid();
        var orderSingle = new NewOrderSingle(
            new ClOrdID(orderId.ToString()),
            new Symbol(order.Symbol),
            GetSide(order.Side),
            new TransactTime(DateTime.UtcNow),
            new OrdType(OrdType.LIMIT)
        );

        orderSingle.SetField(new OrderQty(order.Quantity));
        orderSingle.SetField(new Price(order.Price));

        var sent = Session.SendToTarget(orderSingle, _application.SessionID);

        if (!sent)
            return new OrderResponse("Failed to send order!", orderId, "Error");

        return new OrderResponse("Order received and is being processed.", orderId, "Pending");
    }

    private static Side GetSide(string side)
    {
        return side.Trim().ToLowerInvariant() switch
        {
            "buy" => new Side(Side.BUY),
            "sell" => new Side(Side.SELL),
            _ => throw new ArgumentException("Invalid Side")
        };
    }
}
