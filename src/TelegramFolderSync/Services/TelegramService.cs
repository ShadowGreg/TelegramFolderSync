using Telegram.Bot;

namespace TelegramFolderSync.Services;

public class TelegramService
{
    private readonly TelegramBotClient _client;
    private readonly string _chatId;

    public TelegramService(string botToken, string chatId)
    {
        _client = new TelegramBotClient(botToken);
        _chatId = chatId;
    }

    public async Task<string> GetCurrentUserIdAsync()
    {
        var me = await _client.GetMe();
        return me.Id.ToString();
    }

    public async Task SendFileAsync(string filePath)
    {
        await using var fs = File.OpenRead(filePath);
        await _client.SendDocument(
            chatId: _chatId,
            document: new Telegram.Bot.Types.InputFileStream(fs, Path.GetFileName(filePath)),
            caption: $"Backup {DateTime.Now}"
        );
    }

    public async Task DownloadLatestBackupAsync(string destinationPath)
    {
        var updates = await _client.GetUpdates();
        var docs = updates
            .Select(u => u.Message?.Document)
            .Where(d => d != null && d.FileName!.Contains("backup"))
            .OrderByDescending(d => d.FileName)
            .ToList();

        if (!docs.Any()) return;

        var file = await _client.GetFile(docs.First()!.FileId);
        await using var saveStream = File.OpenWrite(destinationPath);
        await _client.DownloadFile(file.FilePath!, saveStream);
    }
}