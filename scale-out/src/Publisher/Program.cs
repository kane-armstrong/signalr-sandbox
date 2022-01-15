using Hub.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

await Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();

        var hub = configuration.GetConnectionString("hub");
        services.AddHttpClient<HubClient>(client => client.BaseAddress = new Uri(hub));

        services.Configure<GroupOptions>(configuration.GetSection("Groups"));

        services.AddLogging(config =>
        {
            config.AddConsole();
            config.SetMinimumLevel(LogLevel.Debug);
        });

        services.AddHostedService<HubPublisher>();
    })
    .RunConsoleAsync();

public class HubPublisher : IHostedService
{
    private readonly GroupOptions _groupOptions;
    private readonly HubClient _hubClient;
    private readonly ILogger<HubPublisher> _logger;

    public HubPublisher(IOptions<GroupOptions> groupOptions, HubClient hubClient, ILogger<HubPublisher> logger)
    {
        _hubClient = hubClient;
        _logger = logger;
        _groupOptions = groupOptions.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            foreach (var name in _groupOptions.Names)
            {
                _logger.LogDebug($"Greeting group: '{name}'");
                await _hubClient.GreetGroup(name);
                await Pause(cancellationToken);
            }

            _logger.LogDebug("Greeting everyone");
            await _hubClient.GreetEveryone();
            await Pause(cancellationToken);
        }
    }

    private Task Pause(CancellationToken cancellationToken)
    {
        return Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

public class GroupOptions
{
    public string[] Names { get; set; }
}

