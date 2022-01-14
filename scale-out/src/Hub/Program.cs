using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json").AddEnvironmentVariables();
builder.Services.AddSignalR().AddRedis(builder.Configuration.GetConnectionString("redis"));
builder.Services.AddMvcCore();

var app = builder.Build();

app.UseDeveloperExceptionPage();

app.UseRouting();

app.UseEndpoints(endpoints => endpoints.MapHub<GreetingsHub>("greetings"));

app.MapPost("greet/all", async (IHubContext<GreetingsHub> hub) =>
{
    await hub.Clients.All.SendAsync("greet", "hello everyone");
    return Results.NoContent();
});

app.MapPost("greet/group/{group}", async (IHubContext<GreetingsHub> hub, string group) =>
{
    await hub.Clients.Group(group).SendAsync("greet", $"hello {group}");
    return Results.NoContent();
});

app.Run();

public class GreetingsHub : Hub
{
    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }
}