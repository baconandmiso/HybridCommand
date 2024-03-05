using Discord;
using Discord.WebSocket;
using DiscordBotTemplate.Manager;

namespace DiscordBotTemplate.Events;

public class MessageReceivedHandler : IEventHandler
{
    /// <summary>
    ///     イベント登録
    /// </summary>
    public static void Register()
    {
        Program.DiscordSharded.MessageReceived += OnMessageReceived;
    }

    /// <summary>
    ///     メッセージイベント
    /// </summary>
    /// <param name="messageParam">メッセージ</param>
    private static async Task OnMessageReceived(SocketMessage messageParam)
    {
        if (messageParam is not SocketUserMessage message)
            return;

        if (message.Channel.GetChannelType() == ChannelType.DM) // DMは無視
            return;

        if ((message.Author.IsBot || message.Author.IsWebhook) && message.Interaction == null) // BotとWebhookは無視。 インタラクションは通す
            return;

        await CommandManager.ExecuteAsync(message);
    }
}
