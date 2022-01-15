using Hub.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class ScaleoutTests
    {
        [Fact]
        public async Task Clients_receive_messages_regardless_of_which_node_they_are_connected_to()
        {
            var hub1Connection = new HubConnectionBuilder()
                .WithUrl(TestResources.Configuration.GetConnectionString("hub1"))
                .Build();

            var hub2Connection = new HubConnectionBuilder()
                .WithUrl(TestResources.Configuration.GetConnectionString("hub2"))
                .Build();

            await hub1Connection.StartAsync();
            await hub2Connection.StartAsync();

            var hub1ConnectionReceivedDirectMessage = false;
            var hub1ConnectionReceivedGlobalMessage = false;
            var hub2ConnectionReceivedDirectMessage = false;
            var hub2ConnectionReceivedGlobalMessage = false;

            const string groupOnNode1 = "one";
            const string groupOnNode2 = "two";

            hub1Connection.On("greet", (string message) =>
            {
                if (message == $"hello {groupOnNode1}")
                {
                    hub1ConnectionReceivedDirectMessage = true;
                }

                if (message == "hello everyone")
                {
                    hub1ConnectionReceivedGlobalMessage = true;
                }
            });

            hub2Connection.On("greet", (string message) =>
            {
                if (message == $"hello {groupOnNode2}")
                {
                    hub2ConnectionReceivedDirectMessage = true;
                }

                if (message == "hello everyone")
                {
                    hub2ConnectionReceivedGlobalMessage = true;
                }
            });

            await hub1Connection.InvokeCoreAsync("joinGroup", new object[] { groupOnNode1 });
            await hub2Connection.InvokeCoreAsync("joinGroup", new object[] { groupOnNode2 });

            var url = TestResources.Configuration.GetConnectionString("hub1BaseUrl");
            var hub1Client = new HubClient(new HttpClient
            {
                BaseAddress = new Uri(url)
            });

            await hub1Client.GreetGroup(groupOnNode1);
            await hub1Client.GreetGroup(groupOnNode2);
            await hub1Client.GreetEveryone();

            await Task.Delay(TimeSpan.FromSeconds(10));

            Assert.True(hub1ConnectionReceivedDirectMessage);
            Assert.True(hub1ConnectionReceivedGlobalMessage);
            Assert.True(hub2ConnectionReceivedDirectMessage);
            Assert.True(hub2ConnectionReceivedGlobalMessage);

            await hub1Connection.DisposeAsync();
            await hub2Connection.DisposeAsync();
        }
    }
}