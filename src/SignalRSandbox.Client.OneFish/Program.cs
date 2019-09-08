using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace SignalRSandbox.Client.OneFish
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:7076/fish")
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                })
                .Build();

            await connection.StartAsync();

            Console.WriteLine("Starting connection. Press any key to close.");
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, a) =>
            {
                a.Cancel = true;
                cts.Cancel();
            };

            connection.Closed += e =>
            {
                Console.WriteLine("Connection closed with error: {0}", e);

                cts.Cancel();
                return Task.CompletedTask;
            };
            
            connection.On("fishCaught", (string message) =>
            {
                Console.WriteLine($"Fish caught: {message}");
            });

            await connection.InvokeCoreAsync("joinGroup", new object[] { "onefish" }, cts.Token);

            Console.ReadKey();
        }
    }
}