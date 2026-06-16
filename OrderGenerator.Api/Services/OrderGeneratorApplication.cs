using QuickFix;

namespace OrderGenerator.Api.Services;

public class OrderGeneratorApplication : IApplication
{
    public SessionID? SessionID { get; private set; }

    public void OnCreate(SessionID sessionID) => Console.WriteLine($"Sessão criada: {sessionID}");
    public void OnLogon(SessionID sessionID)
    {
        SessionID = sessionID;
        Console.WriteLine($"Logon: {sessionID}");
    }
    public void OnLogout(SessionID sessionID) => Console.WriteLine($"Logout: {sessionID}");
    public void ToAdmin(Message message, SessionID sessionID) { }
    public void FromAdmin(Message message, SessionID sessionID) { }
    public void ToApp(Message message, SessionID sessionID) => Console.WriteLine($"Enviando mensagem: {message}");
    public void FromApp(Message message, SessionID sessionID) => Console.WriteLine($"Mensagem recebida: {message}");
}
