using OrderAccumulator.Services.Interfaces;
using QuickFix;
using QuickFix.Logger;
using QuickFix.Store;

namespace OrderAccumulator.Services;

public class FixAcceptorService : IFixAcceptorService
{
    private readonly string _configFile;
    private IAcceptor? _acceptor;
    private readonly OrderAccumulatorApplication _application;

    public FixAcceptorService(string configFile, OrderAccumulatorApplication application)
    {
        _configFile = configFile;
        _application = application ?? throw new ArgumentNullException(nameof(application));
    }

    public void Start()
    {
        var settings = new SessionSettings(_configFile);
        var application = _application;
        var storeFactory = new FileStoreFactory(settings);
        var logFactory = new FileLogFactory(settings);

        _acceptor = new ThreadedSocketAcceptor(application, storeFactory, settings, logFactory);
        _acceptor.Start();

        Console.WriteLine("Acceptor started.");
    }

    public void Stop()
    {
        _acceptor?.Stop();
        Console.WriteLine("Acceptor stopped.");
    }
}
