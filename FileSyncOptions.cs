public class FileSyncOptions
{
    public const string Options = "Options";

    public string[] WatchDirectories { get; set; } = new string[] {};
    public string[] BackupDirectories { get; set; } = new string[] {};
    public int ExecutionInterval { get; set; }
    public string[] IgnoreFolders { get; set; } = new string[] {};
}
