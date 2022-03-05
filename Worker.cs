namespace CloudFileBackup;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _config;
    private readonly List<FileSynchronizer> _fileSynchronizers;
    private FileSyncOptions fileSyncOptions { get; set; }

    public Worker(ILogger<Worker> logger,IConfiguration config)
    {
        _logger = logger;
        _config = config;
        fileSyncOptions = _config.GetSection(FileSyncOptions.Options).Get<FileSyncOptions>();

        if (fileSyncOptions.WatchDirectories.Length < 1)
        {
            _logger.LogError("WatchDirectories has no values.", fileSyncOptions);
            throw new Exception("WatchDirectories has no values.");
        }

        _fileSynchronizers = new List<FileSynchronizer>();
        for (int i = 0; i < fileSyncOptions.WatchDirectories.Length; i++)
        {
            _fileSynchronizers.Add(
                    new FileSynchronizer(_logger, fileSyncOptions.WatchDirectories[i], fileSyncOptions.BackupDirectories[i], fileSyncOptions.IgnoreFolders)
                    );
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Syncing files at: {time}", DateTimeOffset.Now);
            Parallel.ForEach(_fileSynchronizers, 
                    fileSync => 
                    {
                        fileSync.Synchronize();
                    });

            await Task.Delay(fileSyncOptions.ExecutionInterval, stoppingToken);
        }
    }
}
