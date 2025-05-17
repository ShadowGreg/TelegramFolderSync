using Microsoft.Extensions.Configuration;
using Serilog;

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
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Файл конфигурации не найден: {Path.GetFullPath(path)}");
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(path, optional: false)
            .Build();

        var config = configuration.Get<SyncConfig>();
        if (config is null || string.IsNullOrWhiteSpace(config.TelegramBotToken))
        {
            throw new InvalidOperationException("Не удалось загрузить TelegramBotToken. Проверь файл конфигурации.");
        }

        return config;
    }
}