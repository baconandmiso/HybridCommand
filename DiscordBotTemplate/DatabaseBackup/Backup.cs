using MySql.Data.MySqlClient;
using Serilog;

namespace DiscordBotTemplate.DatabaseBackup;

public class DatabaseBackup
{
    /// <summary>
    ///     指定したデータベースのバックアップをします。 (フル）
    /// </summary>
    /// <param name="database">対象データベース</param>
    /// <param name="stream">出力先</param>
    public static void BackupToStream(string database, ref MemoryStream stream)
    {
        try {
            Program.MySqlConnection.Database = database;

            using var connection = new MySqlConnection(Program.MySqlConnection.ConnectionString);
            connection.Open();

            var mb = new MySqlBackup(connection.CreateCommand());
            mb.ExportToStream(stream);

            connection.Close();
        } catch (Exception ex) {
            Log.Error($"[MySql Backup] {ex.Message}");
        }
    }

    /// <summary>
    ///     データベースの復元を行います。
    /// </summary>
    /// <param name="database">対象データベース</param>
    /// <param name="stream">バックアップデータ</param>
    public static void RestoreToStream(string database, MemoryStream stream)
    {
        try {
            Program.MySqlConnection.Database = database;

            using var connection = new MySqlConnection(Program.MySqlConnection.ConnectionString);
            connection.Open();

            var mb = new MySqlBackup(connection.CreateCommand());
            mb.ImportFromStream(stream);

            connection.Close();
        } catch (Exception ex) {
            Log.Error($"[MySql Restore] {ex.Message}");
        }
    }
}
