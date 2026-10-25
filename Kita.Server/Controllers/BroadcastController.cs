using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class BroadcastController: ControllerBase
    {
        private IHubContext<HealthCheckHub> _hub;
        public BroadcastController(IHubContext<HealthCheckHub> hub)
        {
            _hub = hub;
        }

        [HttpGet]
        public async Task<IActionResult> Update()
        {
            await _hub.Clients.All.SendAsync("Update", "test");
            return Ok("Update message sent.");
        }
    }
}
