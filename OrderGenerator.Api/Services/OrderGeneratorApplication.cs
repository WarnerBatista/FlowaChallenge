using Microsoft.AspNetCore.SignalR;
using OrderGenerator.Api.Dtos;
using OrderGenerator.Api.Hubs;
using QuickFix;

namespace OrderGenerator.Api.Services;

public class OrderGeneratorApplication : IApplication
{
    private readonly IHubContext<OrderHub> _hubContext;

    public OrderGeneratorApplication(IHubContext<OrderHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public SessionID? SessionID { get; private set; }

    public void OnCreate(SessionID sessionID) => Console.WriteLine($"Session created: {sessionID}");
    public void OnLogon(SessionID sessionID)
    {
        SessionID = sessionID;
        Console.WriteLine($"Logon: {sessionID}");
    }
    public void OnLogout(SessionID sessionID) => Console.WriteLine($"Logout: {sessionID}");
    public void ToAdmin(Message message, SessionID sessionID) { }
    public void FromAdmin(Message message, SessionID sessionID) { }
    public void ToApp(Message message, SessionID sessionID) => Console.WriteLine($"Sending message: {message}");

    public void FromApp(Message message, SessionID sessionID)
    {
        if (message is QuickFix.FIX44.ExecutionReport report)
        {
            var orderId = report.OrderID.Value;
            var (status, returnMessage) = report.ExecType.Value switch
            {
                '0' => ("Executed", "Order executed successfully"),
                '8' => ("Rejected", "Order rejected by exposition limit"),
                _ => throw new ArgumentException("Invalid Type")
            };

            var orderResponse = new OrderResponse(returnMessage, orderId, status);

            _hubContext.Clients.Group(orderId).SendAsync("OrderStatusUpdated", orderResponse);
        }
    }
}
