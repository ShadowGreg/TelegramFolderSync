using System.IO.Compression;

namespace TelegramFolderSync.Services;

public class ZipService
{
    public void CreateZip(string sourceFolder, string destinationZip)
    {
        if (File.Exists(destinationZip)) File.Delete(destinationZip);
        ZipFile.CreateFromDirectory(sourceFolder, destinationZip, CompressionLevel.Fastest, true);
    }

    public void ExtractZip(string zipPath, string targetFolder)
    {
        if (Directory.Exists(targetFolder)) Directory.Delete(targetFolder, true);
        ZipFile.ExtractToDirectory(zipPath, targetFolder);
    }
}