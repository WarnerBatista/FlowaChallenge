using Microsoft.AspNetCore.SignalR;
using OrderGenerator.Api.Dtos;
using OrderGenerator.Api.Hubs;
using OrderGenerator.Api.Services.Interfaces;
using QuickFix;
using QuickFix.Fields;

namespace OrderGenerator.Api.Services;

public class OrderGeneratorApplication : IApplication
{
    private readonly IHubContext<OrderHub> _hubContext;
    private readonly ISessionIdProvider _sessionIdProvider;

    public OrderGeneratorApplication(IHubContext<OrderHub> hubContext, ISessionIdProvider sessionIdProvider)
    {
        _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        _sessionIdProvider = sessionIdProvider ?? throw new ArgumentNullException(nameof(sessionIdProvider));
    }

    public void OnCreate(SessionID sessionID) => Console.WriteLine($"Session created: {sessionID}");
    public void OnLogon(SessionID sessionID)
    {
        _sessionIdProvider.SetSessionId(sessionID);
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
                ExecType.NEW => ("Executed", "Order executed successfully"),
                ExecType.REJECTED => ("Rejected", "Order rejected by exposure limit"),
                _ => throw new ArgumentException("Invalid Type")
            };

            var orderResponse = new OrderResponse(returnMessage, orderId, status);

            _hubContext.Clients.Group(orderId).SendAsync("OrderStatusUpdated", orderResponse);
        }
    }
}
