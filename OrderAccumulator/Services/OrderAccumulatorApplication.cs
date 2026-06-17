using QuickFix;
using QuickFix.Fields;

namespace OrderAccumulator.Services;

public class OrderAccumulatorApplication : IApplication
{
    int execID = 0;

    private string GenExecID() { return (++execID).ToString(); }

    public void OnCreate(SessionID sessionID) => Console.WriteLine($"Sessão criada: {sessionID}");
    public void OnLogon(SessionID sessionID) => Console.WriteLine($"Logon: {sessionID}");
    public void OnLogout(SessionID sessionID) => Console.WriteLine($"Logout: {sessionID}");
    public void ToAdmin(Message message, SessionID sessionID) { }
    public void FromAdmin(Message message, SessionID sessionID) { }
    public void ToApp(Message message, SessionID sessionID) => Console.WriteLine($"Enviando mensagem: {message}");

    public void FromApp(Message message, SessionID sessionID)
    {
        Console.WriteLine($"Mensagem recebida: {message}");

        if (message is QuickFix.FIX44.NewOrderSingle order)
        {
            Console.WriteLine("Nova ordem recebida.");

            // Todo: Aplicar lógica de acumulação de ordens aqui, por enquanto apenas respondemos com um ExecutionReport do tipo New.
            Thread.Sleep(5000);

            var execReport = new QuickFix.FIX44.ExecutionReport(
                new OrderID(order.ClOrdID.Value),
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
