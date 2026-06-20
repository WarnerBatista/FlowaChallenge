using QuickFix;

namespace FixCommons;

public class SessionIdProvider : ISessionIdProvider
{
    private SessionID? _sessionId;

    public void SetSessionId(SessionID sessionId) => _sessionId = sessionId;

    public bool SendToTarget(Message order)
    {
        if (_sessionId == null)
            throw new InvalidOperationException("SessionId não foi definido.");

        return Session.SendToTarget(order, _sessionId);
    }
}
