using QuickFix;
using QuickFix.Fields;
using QuickFix.FIX44;
using QuickFix.Logger;
using QuickFix.Store;
using QuickFix.Transport;

namespace OrderGenerator.Api.Services;

public class FixInitiatorService
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

        while (!_initiator.IsLoggedOn)
        {
            Thread.Sleep(1000);
        }

        SendOrder();
    }

    public void Stop()
    {
        _initiator?.Stop();
        Console.WriteLine("Initiator encerrado.");
    }

    public void SendOrder()
    {
        if (_application?.SessionID == null)
        {
            Console.WriteLine("Sessão FIX não está ativa.");
            return;
        }

        var order = new NewOrderSingle(
            new ClOrdID(Guid.NewGuid().ToString()),
            new Symbol("PETR4"),
            new Side(Side.BUY),
            new TransactTime(DateTime.UtcNow),
            new OrdType(OrdType.LIMIT)
        );

        order.SetField(new OrderQty(100));
        order.SetField(new Price(25.00m));

        Session.SendToTarget(order, _application.SessionID);
        Console.WriteLine("Ordem de teste enviada.");
    }
}
