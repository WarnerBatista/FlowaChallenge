using QuickFix;
using QuickFix.Logger;
using QuickFix.Store;

namespace OrderAccumulator.Services;

public class FixAcceptorService
{
    private readonly string _configFile;
    private IAcceptor? _acceptor;

    public FixAcceptorService(string configFile)
    {
        _configFile = configFile;
    }

    public void Start()
    {
        var settings = new SessionSettings(_configFile);
        var application = new OrderAccumulatorApplication();
        var storeFactory = new FileStoreFactory(settings);
        var logFactory = new FileLogFactory(settings);

        _acceptor = new ThreadedSocketAcceptor(application, storeFactory, settings, logFactory);
        _acceptor.Start();

        Console.WriteLine("Acceptor iniciado e aguardando conexões...");
    }

    public void Stop()
    {
        _acceptor?.Stop();
        Console.WriteLine("Acceptor encerrado.");
    }
}
