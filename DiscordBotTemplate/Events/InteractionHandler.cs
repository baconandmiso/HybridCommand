using Discord.Interactions;
using Discord.WebSocket;
using DiscordBotTemplate.Manager;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBotTemplate.Events;

public class InteractionHandler : IEventHandler
{
    /// <summary>
    ///     イベント登録
    /// </summary>
    public static void Register()
    {
        Program.DiscordSharded.InteractionCreated += OnInteractionCreated;
    }

    /// <summary>
    ///     インタラクション作成イベント
    /// </summary>
    /// <param name="interaction">Interaction</param>
    private static async Task OnInteractionCreated(SocketInteraction interaction)
    {
        var scope = Program.ServiceProvider.CreateScope();

        var context = new ShardedInteractionContext(Program.DiscordSharded, interaction); 
        await InteractionManager.InteractionService.ExecuteCommandAsync(context, scope.ServiceProvider);
    }
}
