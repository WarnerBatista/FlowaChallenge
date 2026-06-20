using QuickFix;

namespace FixCommons;

public interface ISessionIdProvider
{
    void SetSessionId(SessionID sessionId);
    bool SendToTarget(Message order);
}
