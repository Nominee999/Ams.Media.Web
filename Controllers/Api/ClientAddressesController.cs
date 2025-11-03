using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ams.Media.Web.Dto;
using Ams.Media.Web.Services;

namespace Ams.Media.Web.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public sealed class ClientAddressesController : ControllerBase
    {
        private readonly IClientService _svc;

        public ClientAddressesController(IClientService svc)
        {
            _svc = svc;
        }

        // GET api/ClientAddresses/list?clientId=100000&type=1
        [HttpGet("list")]
        public async Task<IActionResult> List(int clientId, int? type, CancellationToken ct)
        {
            var rows = await _svc.AddressListAsync(clientId, type, ct);
            return Ok(rows);
        }

        // GET api/ClientAddresses/get?clientId=100000&type=1&start=2025-10-15
        [HttpGet("get")]
        public async Task<IActionResult> Get(int clientId, int type, DateTime start, DateTime? end, CancellationToken ct)
        {
            var dto = await _svc.AddressGetAsync(clientId, type, start, end, ct);
            if (dto == null) return NotFound();

            // ถ้าตัวเรียกเดิมคาดว่าเป็นลิสต์ ให้ห่อเป็นลิสต์สั้น ๆ
            // var list = new[] { dto };
            // return Ok(list);

            return Ok(dto);
        }

        // POST api/ClientAddresses/validate
        [HttpPost("validate")]
        public async Task<IActionResult> Validate([FromBody] ClientAddressDto input,
            int? oldClientId, int? oldType, DateTime? oldStart, CancellationToken ct)
        {
            var oldKey = (oldClientId.HasValue && oldType.HasValue && oldStart.HasValue)
                ? (oldClientId.Value, oldType.Value, oldStart.Value)
                : ((int clientId, int addressType, DateTime startDate)?)null;

            var (ok, message, echo) = await _svc.AddressValidateAsync(input, oldKey, ct);
            if (!ok) return BadRequest(new { message, echo });
            return Ok(echo);
        }

        // POST api/ClientAddresses/save  (create/update อัตโนมัติ)
        [HttpPost("save")]
        public async Task<IActionResult> Save([FromBody] ClientAddressDto input,
            int? oldClientId, int? oldType, DateTime? oldStart, CancellationToken ct)
        {
            var oldKey = (oldClientId.HasValue && oldType.HasValue && oldStart.HasValue)
                ? (oldClientId.Value, oldType.Value, oldStart.Value)
                : ((int clientId, int addressType, DateTime startDate)?)null;

            var (ok, message, _) = await _svc.AddressValidateAsync(input, oldKey, ct);
            if (!ok) return BadRequest(new { message });

            bool saved = oldKey is null
                ? await _svc.AddressCreateAsync(input, ct)
                : await _svc.AddressUpdateAsync(oldKey.Value.clientId, oldKey.Value.addressType, oldKey.Value.startDate, input, ct);

            return saved ? Ok() : StatusCode(500, new { message = "Save failed" });
        }

        // DELETE api/ClientAddresses/delete?clientId=100000&type=1&start=2025-10-15
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int clientId, int type, DateTime start, DateTime? end, CancellationToken ct)
        {
            var ok = await _svc.AddressDeleteAsync(clientId, type, start, end, ct);
            return ok ? Ok() : StatusCode(500, new { message = "Delete failed" });
        }
    }
}
