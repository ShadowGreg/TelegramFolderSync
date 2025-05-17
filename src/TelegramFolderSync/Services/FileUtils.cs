using System.Runtime.InteropServices;

namespace TelegramFolderSync.Services;

public static class FileUtils
{
    public static bool IsFileLocked(IOException exception)
    {
        int errorCode = Marshal.GetHRForException(exception) & ((1 << 16) - 1);
        return errorCode == 32 || errorCode == 33;
    }
}