using FixCommons;
using OrderAccumulator.Services.Interfaces;
using QuickFix;

namespace OrderAccumulator.Services;

public class OrderAccumulatorApplication : IApplication
{
    private readonly Dictionary<char, IExposureProcessor> _exposureProcessors;
    private readonly ISessionIdProvider _sessionIdProvider;

    public OrderAccumulatorApplication(Dictionary<char, IExposureProcessor> exposureProcessors, ISessionIdProvider sessionIdProvider)
    {
        _exposureProcessors = exposureProcessors ?? throw new ArgumentNullException(nameof(exposureProcessors));
        _sessionIdProvider = sessionIdProvider ?? throw new ArgumentNullException(nameof(sessionIdProvider));
    }

    public void OnCreate(SessionID sessionID) => Console.WriteLine($"Session created: {sessionID}");
    public void OnLogon(SessionID sessionID)
    {
        Console.WriteLine($"Logon: {sessionID}");
        _sessionIdProvider.SetSessionId(sessionID);
    }
    public void OnLogout(SessionID sessionID) => Console.WriteLine($"Logout: {sessionID}");
    public void ToAdmin(Message message, SessionID sessionID) { }
    public void FromAdmin(Message message, SessionID sessionID) { }
    public void ToApp(Message message, SessionID sessionID) => Console.WriteLine($"Sending message: {message}");

    public void FromApp(Message message, SessionID sessionID)
    {
        if (message is QuickFix.FIX44.NewOrderSingle order)
        {
            //Simulate processing time
            Thread.Sleep(5000);

            var execReport = _exposureProcessors[order.Side.Value].Process(order);

            _sessionIdProvider.SendToTarget(execReport);
            Console.WriteLine("ExecutionReport sent.");
        }
    }
}
