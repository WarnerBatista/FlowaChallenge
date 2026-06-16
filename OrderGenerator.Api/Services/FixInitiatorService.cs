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

        Console.WriteLine("Initiator iniciado e aguardando logon...");
    }

    public void Stop()
    {
        _initiator?.Stop();
        Console.WriteLine("Initiator encerrado.");
    }

    public void SendOrder(OrderDto order)
    {
        if (_application?.SessionID == null)
        {
            Console.WriteLine("Sessão FIX não está ativa.");
            return;
        }

        var orderSingle = new NewOrderSingle(
            new ClOrdID(Guid.NewGuid().ToString()),
            new Symbol(order.Symbol),
            GetSide(order.Side),
            new TransactTime(DateTime.UtcNow),
            new OrdType(OrdType.LIMIT)
        );

        orderSingle.SetField(new OrderQty(order.Quantity));
        orderSingle.SetField(new Price(order.Price));

        Session.SendToTarget(orderSingle, _application.SessionID);
        Console.WriteLine("Ordem de teste enviada.");
    }

    private static Side GetSide(string side)
    {
        return side.Trim().ToLowerInvariant() switch
        {
            "buy" => new Side(Side.BUY),
            "sell" => new Side(Side.SELL),
            _ => throw new ArgumentException("Side inválido")
        };
    }
}
