using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using Serilog;

namespace DiscordBotTemplate.DatabaseBackup;

public static class GooglDrive
{
    /// <summary>
    ///     Google APIと通信する際にスコープするもの
    /// </summary>
    private static string[] _scopes = { DriveService.Scope.Drive };

    /// <summary>
    ///     ファイルをアップロードします。
    /// </summary>
    /// <param name="stream">メモリに保存されているデータ</param>
    /// <param name="filename">ファイル名</param>
    public static void UploadFileFromStream(Stream stream, string filename)
    {
        var fs = new FileStream(Config.Get("GoogleAPI:DriveKey"), FileMode.Open, FileAccess.Read);
        GoogleCredential credential;

        try {
            credential = GoogleCredential.FromStream(fs).CreateScoped(_scopes);
        } finally {
            fs.Close();
        }

        var initializer = new BaseClientService.Initializer();
        initializer.HttpClientInitializer = credential;
        initializer.ApplicationName = Config.Get("GoogleAPI:ApplicationName");

        var service = new DriveService(initializer);
        IUploadProgress uploadProgress;

        try {
            var meta = new Google.Apis.Drive.v3.Data.File();
            meta.Name = Path.GetFileName(filename);
            meta.MimeType = "Content-Type: application/octet-stream";
            meta.Parents = new List<string>() { Config.Get("GoogleDrive:Path") };
            
            var request = service.Files.Create(meta, stream, "Content-Type: application/octet-stream");
            request.Fields = "id, name";
            uploadProgress = request.Upload();

            Log.Information($"[GoogleDrive Upload] 容量: {uploadProgress.BytesSent} byte, status: {uploadProgress.Status}");
        } catch (Exception ex) {
            Log.Error($"[GoogleDrive Upload] {ex.Message}");
        } finally {
            stream.Close();
        }
    }

    /// <summary>
    ///     ファイルをストリームにダウンロードします。
    /// </summary>
    /// <param name="fileId">GoogleDriveのファイルId</param>
    /// <returns>ダウンロードしたデータ</returns>
    public static Stream? DownloadFileToStream(string fileId)
    {
        var fs = new FileStream(Config.Get("GoogleAPI:DriveKey"), FileMode.Open, FileAccess.Read);
        GoogleCredential credential;

        try {
            credential = GoogleCredential.FromStream(fs).CreateScoped(_scopes);
        } finally {
            fs.Close();
        }

        var initializer = new BaseClientService.Initializer();
        initializer.HttpClientInitializer = credential;
        initializer.ApplicationName = Config.Get("GoogleAPI:ApplicationName");

        var service = new DriveService(initializer);

        try {
            var stream = new MemoryStream();

            var request = service.Files.Get(fileId);
            request.Download(stream);

            Log.Information($"[GoogleDrive Download] ダウンロードが完了しました。 file_name: {request.Execute().Name}");
            return stream;
        } catch (Exception ex) {
            Log.Error($"[GoogleDrive Download] {ex.Message}");
        }

        return null;
    }

    /// <summary>
    ///     フォルダー内にあるファイルを取得します。
    /// </summary>
    /// <param name="folderId">フォルダーID</param>
    /// <returns></returns>
    public static string[]? GetFilesInFolder(string folderId)
    {
        var fs = new FileStream(Config.Get("GoogleAPI:DriveKey"), FileMode.Open, FileAccess.Read);
        GoogleCredential credential;

        try {
            credential = GoogleCredential.FromStream(fs).CreateScoped(_scopes);
        } finally {
            fs.Close();
        }

        var initializer = new BaseClientService.Initializer();
        initializer.HttpClientInitializer = credential;
        initializer.ApplicationName = Config.Get("GoogleAPI:ApplicationName");

        var service = new DriveService(initializer);

        FilesResource.ListRequest listRequest = service.Files.List();
        listRequest.PageSize = 1000;
        listRequest.Fields = "nextPageToken, files(id, name)";
        listRequest.Q = string.Format($"mimeType != 'application/vnd.google-apps.folder' and '{folderId}' in parents");

        var files = listRequest.Execute().Files;
        if (files != null && files.Count > 0)
        {
            string[] result = new string[files.Count];
            int index = 0;
            foreach (var file in files)
            {
                result[index] = $"{file.Name} / {file.Id}";
                index++;
            }

            return result;
        }

        return null;
    }
}
