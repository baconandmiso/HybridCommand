namespace DiscordBotTemplate.Events;

public interface IEventHandler
{
    /// <summary>
    ///     イベント登録するために使う
    /// </summary>
    public static abstract void Register();
}
