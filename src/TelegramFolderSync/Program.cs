using Serilog;
using TelegramFolderSync.Services;
using TelegramFolderSync.Models;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
    .MinimumLevel.Debug()
    .Enrich.FromLogContext()
    .CreateLogger();

try
{
    Log.Information("Загрузка конфигурации...");
    var config = SyncConfig.Load();

    var tg = new TelegramService(config.TelegramBotToken, config.TelegramChatId);
    var zip = new ZipService();

    string userId = await tg.GetCurrentUserIdAsync();
    string zipFilePath = Path.Combine(Path.GetTempPath(), $"backup_{userId}.zip");

    var syncService = new SyncService(tg, zip, config, zipFilePath);
    await syncService.RunLoopAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Критическая ошибка при запуске");
}
finally
{
    Log.CloseAndFlush();
}