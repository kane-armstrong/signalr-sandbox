using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SignalRSandbox.Web
{
    public class FishHub : Hub
    {
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
    }
}