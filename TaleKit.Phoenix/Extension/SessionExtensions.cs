using TaleKit.Game;

namespace TaleKit.Phoenix.Extension;

public static class SessionExtensions
{
    public static bool IsReady(this Session session)
    {
        return session.Character.Map is not null && session.Character.Skills.Count != 0;
    }
}