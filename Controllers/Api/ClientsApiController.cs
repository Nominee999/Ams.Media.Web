using System.Threading.Tasks;
using Ams.Media.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ams.Media.Web.Controllers.Api
{
    [ApiController]
    [Route("api/clients")]
    public class ClientsApiController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientsApiController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetList([FromQuery] string? from, [FromQuery] string? to, [FromQuery] string? show)
        {
            // ถ้าไม่ส่ง show มา ใช้ cookie ShowData (A|C)
            var cookieShow = Request.Cookies.TryGetValue("ShowData", out var c) ? c : null;
            var effectiveShow = (show ?? cookieShow ?? "C").ToUpperInvariant();
            if (effectiveShow != "A" && effectiveShow != "C") effectiveShow = "C";

            var data = await _clientService.SearchAsync(from ?? "", to ?? "", effectiveShow);
            return Ok(data);
        }
    }
}
