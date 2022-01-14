using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder().AddEnvironmentVariables().AddJsonFile("appsettings.json").Build();
var connection = new HubConnectionBuilder()
    .WithUrl(configuration.GetConnectionString("hub"))
    .ConfigureLogging(logging => logging.AddConsole())
    .Build();

await connection.StartAsync();

Console.WriteLine("Starting connection. Press any key to close.");

var cts = new CancellationTokenSource();

Console.CancelKeyPress += (_, args) =>
{
    args.Cancel = true;
    cts.Cancel();
};

connection.Closed += e =>
{
    Console.WriteLine("Connection closed with error: {0}", e);
    cts.Cancel();
    return Task.CompletedTask;
};

connection.On("greet", (string message) => Console.WriteLine($"Greeting received: {message}"));

await connection.InvokeCoreAsync("joinGroup", new object[] { "one" }, cts.Token);

Console.ReadKey();