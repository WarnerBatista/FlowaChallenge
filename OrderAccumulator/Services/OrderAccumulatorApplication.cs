using QuickFix;
using QuickFix.Fields;
using QuickFix.FIX44;

namespace OrderAccumulator.Services;

public class OrderAccumulatorApplication : IApplication
{
    int orderID = 0;
    int execID = 0;

    private string GenOrderID() { return (++orderID).ToString(); }
    private string GenExecID() { return (++execID).ToString(); }

    public void OnCreate(SessionID sessionID) => Console.WriteLine($"Sessão criada: {sessionID}");
    public void OnLogon(SessionID sessionID) => Console.WriteLine($"Logon: {sessionID}");
    public void OnLogout(SessionID sessionID) => Console.WriteLine($"Logout: {sessionID}");
    public void ToAdmin(QuickFix.Message message, SessionID sessionID) { }
    public void FromAdmin(QuickFix.Message message, SessionID sessionID) { }
    public void ToApp(QuickFix.Message message, SessionID sessionID) => Console.WriteLine($"Enviando mensagem: {message}");

    public void FromApp(QuickFix.Message message, SessionID sessionID)
    {
        Console.WriteLine($"Mensagem recebida: {message}");

        if (message is NewOrderSingle order)
        {
            Console.WriteLine("Nova ordem recebida.");

            // Todo: Aplicar lógica de acumulação de ordens aqui, por enquanto apenas respondemos com um ExecutionReport do tipo New.

            var execReport = new ExecutionReport(
                new OrderID(GenOrderID()),
                new ExecID(GenExecID()),
                new ExecType(ExecType.NEW),
                new OrdStatus(OrdStatus.NEW),
                order.Symbol,
                order.Side,
                new LeavesQty(0),
                new CumQty(order.OrderQty.Value),
                new AvgPx(order.Price.Value)
            );

            Session.SendToTarget(execReport, sessionID);
            Console.WriteLine("ExecutionReport enviado (New).");
        }
    }
}
