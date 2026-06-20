using FixCommons;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderAccumulator.Services;
using OrderAccumulator.Services.Interfaces;
using QuickFix.Fields;

var host = Host.CreateDefaultBuilder(args)
.ConfigureServices(services =>
{
    services.AddSingleton<ISessionIdProvider, SessionIdProvider>();
    services.AddSingleton<IExposureAccumulator, ExposureAccumulator>();
    services.AddKeyedTransient<IExposureProcessor, BuyExposureProcessor>(Side.BUY);
    services.AddKeyedTransient<IExposureProcessor, SellExposureProcessor>(Side.SELL);

    services.AddSingleton(sp =>
    {
        return new OrderAccumulatorApplication(new Dictionary<char, IExposureProcessor>
        {
            { Side.BUY, sp.GetRequiredKeyedService<IExposureProcessor>(Side.BUY) },
            { Side.SELL, sp.GetRequiredKeyedService<IExposureProcessor>(Side.SELL) }
        },
        sp.GetRequiredService<ISessionIdProvider>());
    });

    services.AddSingleton<IFixAcceptorService>(sp =>
    {
        return new FixAcceptorService("Config/Acceptor.cfg", sp.GetRequiredService<OrderAccumulatorApplication>());
    });
})
.Build();

Console.WriteLine("Starting Order Accumulator (Acceptor FIX 4.4)...");

var service = host.Services.GetRequiredService<IFixAcceptorService>();
service.Start();

Console.WriteLine("Press ENTER to stop.");
Console.ReadLine();

service.Stop();
