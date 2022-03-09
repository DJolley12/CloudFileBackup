public class FileSynchronizer
{
    private readonly ILogger _logger;

    private string source { get; }
    private string destination { get; }
    private string[] _ignoreFolders { get; }
    private string lastDirectory { get; set; } = "";
    private string lastFile { get; set; } = "";

    public FileSynchronizer(ILogger logger, string sourceString, string destinationString, string[] ignoreFolders)
    {
        if (!Directory.Exists(sourceString))
        {
            throw new Exception($"Invalid source directory {sourceString}");
        }

        _logger = logger;
        source = sourceString;
        destination = destinationString;
        _ignoreFolders = ignoreFolders;
    }

    public Task Synchronize()
    {
        try
        {
            var sourceDirectoryInfo = new DirectoryInfo(source);
            var destDirectoryInfo = new DirectoryInfo(destination);

            SynchronizeRecursively(sourceDirectoryInfo, destDirectoryInfo);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, $"An exception occurred. Last file: {lastFile}\nLast directory: {lastDirectory}");
        }

        return Task.CompletedTask;
    }

    private void SynchronizeRecursively(DirectoryInfo sourceDirectoryInfo, DirectoryInfo destDirectoryInfo)
    {
        if (_ignoreFolders.Contains(sourceDirectoryInfo.Name))
        {
            return;
        }

        SyncFiles(sourceDirectoryInfo, destDirectoryInfo);

        foreach (var sourceDir in sourceDirectoryInfo.GetDirectories())
        {
            var destDir = destDirectoryInfo.GetDirectories().FirstOrDefault(dd => dd.Name == sourceDir.Name);
            lastDirectory = sourceDir.FullName;

            if (destDir is null)
            {
                if (_ignoreFolders.Contains(sourceDirectoryInfo.Name))
                {
                    continue;
                }

                destDir = destDirectoryInfo.CreateSubdirectory(sourceDir.Name);
            }

            SynchronizeRecursively(sourceDir, destDir);
        }
    }

    private void SyncFiles(DirectoryInfo sourceDirectoryInfo, DirectoryInfo destDirectoryInfo)
    {
        foreach (var sourceFileInfo in sourceDirectoryInfo.GetFiles())
        {
            lastFile = sourceFileInfo.FullName;
            var existingFile = destDirectoryInfo.GetFiles().FirstOrDefault(f => f.Name == sourceFileInfo.Name);

            if (existingFile is null)
            {
                sourceFileInfo.CopyTo(Path.Combine(destDirectoryInfo.FullName, sourceFileInfo.Name));
            }
            else if (sourceFileInfo.LastWriteTime < existingFile.LastWriteTime)
            {
                sourceFileInfo.CopyTo(Path.Combine(destDirectoryInfo.FullName, sourceFileInfo.Name), true);
            }
        }
    }
}
