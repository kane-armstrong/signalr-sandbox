using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace SignalRSandbox.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly IHubContext<FishHub> _hub;

        public NotificationsController(IHubContext<FishHub> hub)
        {
            _hub = hub;
        }
        
        [HttpPost("allfish")]
        public async Task<IActionResult> AllFish()
        {
            await _hub.Clients.All.SendAsync("fishCaught", "its a red one");
            return Ok();
        }

        [HttpPost("onefish")]
        public async Task<IActionResult> OneFish()
        {
            await _hub.Clients.Group("onefish").SendAsync("fishCaught", "its a red one");
            return Ok();
        }
        
        [HttpPost("twofish")]
        public async Task<IActionResult> TwoFish()
        {
            await _hub.Clients.Group("twofish").SendAsync("fishCaught", "its a blue one");
            return Ok();
        }
    }
}