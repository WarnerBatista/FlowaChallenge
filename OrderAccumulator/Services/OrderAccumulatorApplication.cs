using OrderAccumulator.Services.Interfaces;
using QuickFix;

namespace OrderAccumulator.Services;

public class OrderAccumulatorApplication : IApplication
{
    private readonly Dictionary<char, IExposureProcessor> _exposureProcessors;

    public OrderAccumulatorApplication(Dictionary<char, IExposureProcessor> exposureProcessors)
    {
        _exposureProcessors = exposureProcessors ?? throw new ArgumentNullException(nameof(exposureProcessors));
    }

    public void OnCreate(SessionID sessionID) => Console.WriteLine($"Session created: {sessionID}");
    public void OnLogon(SessionID sessionID) => Console.WriteLine($"Logon: {sessionID}");
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

            var execReport = _exposureProcessors[order.Side.Value].Process(order) ?? throw new InvalidOperationException("No processor found for the order");

            Session.SendToTarget(execReport, sessionID);
            Console.WriteLine("ExecutionReport sent.");
        }
    }
}
