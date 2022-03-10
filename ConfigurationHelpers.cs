using System.Diagnostics;

namespace CloudFileBackup
{
    public static class ConfigurationHelpers
    {
        public static string GetBasePath()
        {
            using var processModule = Process.GetCurrentProcess().MainModule;
            return Path.GetDirectoryName(processModule?.FileName);
        }
    }
}