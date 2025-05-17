using Serilog;
using TelegramFolderSync.Models;

namespace TelegramFolderSync.Services;

public class SyncService
{
    private readonly TelegramService _telegramService;
    private readonly ZipService _zipService;
    private readonly SyncConfig _config;
    private readonly string _zipFilePath;

    public SyncService(
        TelegramService telegramService,
        ZipService zipService,
        SyncConfig config,
        string zipFilePath)
    {
        _telegramService = telegramService;
        _zipService = zipService;
        _config = config;
        _zipFilePath = zipFilePath;
    }

    public async Task RunLoopAsync(CancellationToken cancellationToken = default)
    {
        Log.Information("Служба синхронизации запущена...");

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                Log.Information("🕒 {Timestamp} Начало синхронизации", DateTime.Now);

                _zipService.CreateZip(_config.BackupFolderPath, _zipFilePath);
                await _telegramService.SendFileAsync(_zipFilePath);

                await _telegramService.DownloadLatestBackupAsync(_zipFilePath);
                _zipService.ExtractZip(_zipFilePath, _config.RestoreFolderPath);

                Log.Information("✅ Синхронизация завершена");
            }
            catch (IOException ex) when (FileUtils.IsFileLocked(ex))
            {
                Log.Warning("⚠️ Файл заблокирован: {Message}", ex.Message);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "❌ Ошибка синхронизации");
            }

            await Task.Delay(TimeSpan.FromMinutes(_config.SyncIntervalMinutes), cancellationToken);
        }
    }
}