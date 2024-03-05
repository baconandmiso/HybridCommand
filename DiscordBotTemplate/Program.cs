using Discord.Interactions;
using Discord.WebSocket;
using DiscordBotTemplate.Events;
using DiscordBotTemplate.Manager;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;

namespace DiscordBotTemplate;

public class Program
{
    /// <summary>
    ///     DiscordSocket通信の設定
    /// </summary>
    static readonly DiscordSocketConfig _discordSocketConfig = new DiscordSocketConfig {
        AlwaysDownloadUsers = true,
        UseInteractionSnowflakeDate = false,
        LogGatewayIntentWarnings = false,
        SuppressUnknownDispatchWarnings = false,
        MessageCacheSize = 1024,
        GatewayIntents = Discord.GatewayIntents.All
    };

    /// <summary>
    ///     DiscordShardedClient
    /// </summary>
    public static readonly DiscordShardedClient DiscordSharded = new DiscordShardedClient(_discordSocketConfig);
    
    /// <summary>
    ///     Service関連
    /// </summary>
    public static readonly IServiceProvider ServiceProvider = new ServiceCollection()
        .AddSingleton(DiscordSharded)
        .AddSingleton<InteractionService>()
        .BuildServiceProvider();

    /// <summary>
    ///     MySqlサーバー接続設定
    /// </summary>
    public static readonly MySqlConnectionStringBuilder MySqlConnection = new MySqlConnectionStringBuilder() {
        Server = Config.Get("MySql:HostName"),
        Port = Config.GetT<uint>("MySql:Port"),
        UserID = Config.Get("MySql:UserName"),
        Password = Config.Get("MySql:Password"),
        CharacterSet = "utf8"
    };

    /// <summary>
    ///     Main
    /// </summary>
    static async Task Main()
    {
        Log.Register();
        MessageReceivedHandler.Register();

        await CommandManager.InitializeAsync();
        await InteractionManager.InitializeAsync();

        await DiscordSharded.LoginAsync(Discord.TokenType.Bot, Config.Get("DiscordBot:Token"));
        await DiscordSharded.StartAsync();

        await Task.Delay(-1);
    }
}
