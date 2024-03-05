using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;

namespace DiscordBotTemplate.Manager;

public static class CommandManager
{
    /// <summary>
    ///     Discord Commandが入ってる。 作ることもできる。
    /// </summary>
    public static readonly CommandService CommandService = new CommandService();

    /// <summary>
    ///     コマンドサービスを初期化する
    /// </summary>
    public static async Task InitializeAsync()
    {
        await LoadModulesAsync();
    }

    /// <summary>
    ///     アセンブリ内にあるモジュールを読み込む
    /// </summary>
    /// <returns></returns>
    public static async Task LoadModulesAsync()
    {
        await CommandService.AddModulesAsync(Assembly.GetEntryAssembly(), Program.ServiceProvider);
    }

    /// <summary>
    ///     読み込んだモジュールを削除する
    /// </summary>
    public static void RemoveModules()
    {
        _ = CommandService.Modules.Select(module => Task.FromResult(CommandService.RemoveModuleAsync(module)));
    }

    /// <summary>
    ///     コマンド実行
    /// </summary>
    /// <param name="message">ユーザーから送られたメッセージ</param>
    /// <returns>実行結果</returns>
    public static async Task<IResult?> ExecuteAsync(SocketUserMessage message)
    {
        int argPos = 0; // 開始位置

        if (!message.HasStringPrefix(Config.Get("DiscordBot:CommandPrefix"), ref argPos)) // 実行条件 
            return null;

        var context = new ShardedCommandContext(Program.DiscordSharded, message);
        return await CommandService.ExecuteAsync(context, argPos, Program.ServiceProvider);
    }
}
