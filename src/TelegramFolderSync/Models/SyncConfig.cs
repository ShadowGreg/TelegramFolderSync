using System.Text.Json;

namespace TelegramFolderSync.Models;

public class SyncConfig
{
    public string TelegramBotToken { get; set; }
    public string TelegramChatId { get; set; }
    public string BackupFolderPath { get; set; }
    public string RestoreFolderPath { get; set; }

    public int SyncIntervalMinutes { get; set; } = 10;


    public static SyncConfig Load(string path = "appsettings.json")
    {
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<SyncConfig>(json)!;
    }
}