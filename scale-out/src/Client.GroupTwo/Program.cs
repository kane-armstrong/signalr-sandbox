using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

await Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();

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
    private const string Group = "two";
    private readonly HubConnection _hubConnection;
    private readonly ILogger<HubListener> _logger;

    public HubListener(HubConnection hubConnection, ILogger<HubListener> logger)
    {
        _hubConnection = hubConnection;
        _logger = logger;
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

        await _hubConnection.InvokeCoreAsync("joinGroup", new object[] { Group }, cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _hubConnection.DisposeAsync();
    }
}