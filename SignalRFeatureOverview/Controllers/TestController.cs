using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalRFeatureOverview.Hub;

namespace SignalRFeatureOverview.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IHubContext<CustomHub> _customhub;

        public TestController(IHubContext<CustomHub> cutomhub)
        {
            this._customhub = cutomhub;
        }

        [HttpGet("/send")]
        public async Task<IActionResult> SendData() {
            await this._customhub
                            .Clients
                            .All
                            .SendAsync("client_function_name",new Data(100,"Dummy Data"));
            return Ok();
        }
    }
}
