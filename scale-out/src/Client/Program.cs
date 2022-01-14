using Microsoft.AspNetCore.SignalR.Client;
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

        services.Configure<GroupOptions>(configuration.GetSection("Group"));
        services.AddScoped(_ =>
        {
            return new HubConnectionBuilder()
                .WithUrl(configuration.GetConnectionString("hub"))
                .ConfigureLogging(logging => logging.AddConsole())
                .Build();
        });

        services.AddHostedService<HubListener>();
    })
    .RunConsoleAsync();

public class HubListener : IHostedService
{
    private readonly GroupOptions _groupOptions;
    private readonly HubConnection _hubConnection;
    private readonly ILogger<HubListener> _logger;

    public HubListener(HubConnection hubConnection, ILogger<HubListener> logger, IOptions<GroupOptions> groupOptions)
    {
        _hubConnection = hubConnection;
        _logger = logger;
        _groupOptions = groupOptions.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _hubConnection.StartAsync(cancellationToken);

        _hubConnection.Closed += e =>
        {
            _logger.LogDebug("Connection closed with error: {0}", e);
            return Task.CompletedTask;
        };

        _hubConnection.On("greet", (string message) => Console.WriteLine($"Greeting received: {message}"));

        await _hubConnection.InvokeCoreAsync("joinGroup", new object[] { _groupOptions.Name }, cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _hubConnection.DisposeAsync();
    }
}

public class GroupOptions
{
    public string Name { get; set; }
}