using Discord;
using Serilog;
using Serilog.Events;

namespace DiscordBotTemplate.Events;

public class Log : IEventHandler
{
    /// <summary>
    ///     イベント登録
    /// </summary>
    public static void Register()
    {
        Serilog.Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(Config.Get("LogFile:Path"))
            .CreateLogger();

        Quartz.Logging.LogProvider.IsDisabled = true; // タスクスケジューラー ログ無効

        Program.DiscordSharded.Log += OnLog;
    }

    /// <summary>
    ///     イベント本体
    /// </summary>
    /// <param name="message">ログ内容</param>
    private static async Task OnLog(LogMessage message)
    {
        LogEventLevel level = message.Severity switch {
            LogSeverity.Critical => LogEventLevel.Fatal,
            LogSeverity.Warning => LogEventLevel.Warning,
            LogSeverity.Info => LogEventLevel.Information,
            LogSeverity.Verbose => LogEventLevel.Verbose,
            LogSeverity.Debug => LogEventLevel.Debug,
            _ => LogEventLevel.Information
        };

        Serilog.Log.Write(level, message.Exception, $"[{message.Source}] {message.Message}");
        await Task.CompletedTask;
    }
}
