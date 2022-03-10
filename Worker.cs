namespace CloudFileBackup;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _config;
    private readonly List<FileSynchronizer> _fileSynchronizers;
    private FileSyncOptions _fileSyncOptions { get; set; }

    public Worker(ILogger<Worker> logger, IConfiguration config, FileSyncOptions fileSyncOptions)
    {
        _logger = logger;
        _config = config;
        _fileSyncOptions = fileSyncOptions;

        if (_fileSyncOptions is null)
        {
            _logger.LogError($"{nameof(_fileSyncOptions)} is null");
            throw new Exception($"{nameof(_fileSyncOptions)} is null");
        }

        if (_fileSyncOptions.WatchDirectories.Length < 1)
        {
            _logger.LogError("WatchDirectories has no values.", _fileSyncOptions);
            throw new Exception("WatchDirectories has no values.");
        }

        _fileSynchronizers = new List<FileSynchronizer>();
        for (int i = 0; i < _fileSyncOptions.WatchDirectories.Length; i++)
        {
            _fileSynchronizers.Add(
                    new FileSynchronizer(_logger, _fileSyncOptions.WatchDirectories[i], _fileSyncOptions.BackupDirectories[i], _fileSyncOptions.IgnoreFolders)
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

            await Task.Delay(_fileSyncOptions.ExecutionInterval, stoppingToken);
        }
    }
}
