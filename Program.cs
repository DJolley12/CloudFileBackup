using System.Reflection;
using CloudFileBackup;

var config = new ConfigurationBuilder()
    .SetBasePath(ConfigurationHelpers.GetBasePath())
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

var options = config.GetSection(FileSyncOptions.Options).Get<FileSyncOptions>();

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((hostContext, services) =>
{
    services.AddHostedService<Worker>();
    // services.ConfigureOptions<FileSyncOptions>();
    services.AddOptions<FileSyncOptions>(FileSyncOptions.Options);
    // services.AddScoped<IConfiguration>(_ => config);
    services.AddScoped<FileSyncOptions>(_ => options);
    services.AddScoped<IConfigurationRoot>(_ => config);
});

var host = builder.Build();

await host.RunAsync();
