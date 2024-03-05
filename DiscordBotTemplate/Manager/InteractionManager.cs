using Discord.Interactions;
using System.Reflection;

namespace DiscordBotTemplate.Manager;

public static class InteractionManager
{
    /// <summary>
    ///     Discord Application Command が入ってる。
    /// </summary>
    public static readonly InteractionService InteractionService = new InteractionService(Program.DiscordSharded);
    
    /// <summary>
    ///     Interactionサービスの初期化
    /// </summary>
    public static async Task InitializeAsync()
    {
        await LoadModulesAsync();
    }

    /// <summary>
    ///     アセンブリ内にあるモジュールを読み込む
    /// </summary>
    public static async Task LoadModulesAsync()
    {
        await InteractionService.AddModulesAsync(Assembly.GetEntryAssembly(), Program.ServiceProvider);
    }

    /// <summary>
    ///     読み込んだモジュールを削除する
    /// </summary>
    public static void RemoveModules()
    {
        _ = InteractionService.Modules.Select(InteractionService.RemoveModuleAsync);
    }
}
