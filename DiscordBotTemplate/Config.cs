using Microsoft.Extensions.Configuration;

namespace DiscordBotTemplate;

public static class Config
{
    /// <summary>
    ///     key / valueを格納
    /// </summary>
    private static readonly IConfiguration _configuration;

    /// <summary>
    ///     key / valueをセット (Iniファイルを読み込む)
    /// </summary>
    static Config()
    {
        _configuration = new ConfigurationBuilder().AddIniFile("./conf.d/discordbot.conf").Build();
    }

    /// <summary>
    ///    値を取得します。 
    /// </summary>
    /// <param name="key">取得したい値に対応するキー</param>
    /// <returns>値 (string)</returns>
    public static string Get(string key)
    {
        var value = _configuration.GetSection(key).Value;
        if (value == null)
            return "";

        return value;
    }

    /// <summary>
    ///     値を取得します
    /// </summary>
    /// <typeparam name="T">返すときの型</typeparam>
    /// <param name="key">取得したい値に対応するキー</param>
    /// <returns>値 (指定した型)</returns>
    public static T GetT<T>(string key) where T : IParsable<T>
    {
        return T.Parse(_configuration.GetSection(key).Value!, null);
    }
}
