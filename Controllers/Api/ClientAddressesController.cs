// D:\VS2022\Ams.Media.Web\Controllers\Api\ClientAddressesController.cs

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Ams.Media.Web.Dto;
// ✅ ใช้ IClientService จาก namespace ที่ลงทะเบียน DI จริง
using Ams.Media.Web.Services;


namespace Ams.Media.Web.Controllers.Api
{
    [ApiController]
    [Route("api/clients/{clientId:int}/addresses")]
    public class ClientAddressesController : ControllerBase
    {
        private readonly IClientService _svc;

        public ClientAddressesController(IClientService svc)
        {
            _svc = svc;
        }

        // GET: api/clients/{clientId}/addresses?type=10
        [HttpGet]
        public async Task<IActionResult> List(int clientId, [FromQuery] int? type, CancellationToken ct)
        {
            var list = await _svc.AddressListAsync(clientId, type, ct);
            return Ok(list);
        }

        // GET: api/clients/{clientId}/addresses/{addressType}?start=2025-01-01&end=2025-12-31
        [HttpGet("{addressType:int}")]
        public async Task<IActionResult> Get(
            int clientId,
            int addressType,
            [FromQuery] DateTime start,
            [FromQuery] DateTime? end,
            CancellationToken ct)
        {
            var dto = await _svc.AddressGetAsync(clientId, addressType, start, end, ct);
            if (dto is null) return NotFound();
            return Ok(dto);
        }

        // POST: api/clients/{clientId}/addresses/validate
        // body: ClientAddressDto
        [HttpPost("validate")]
        public async Task<IActionResult> ValidateCreate(
            int clientId,
            [FromBody] ClientAddressDto dto,
            CancellationToken ct)
        {
            // ยืนยัน clientId ให้ตรง
            dto.ClientId = clientId;

            // ⚠️ ใส่ type ชัดเจนใน deconstruction ตัดปัญหา Cannot infer…
            (bool ok, string? message, System.Collections.Generic.IReadOnlyList<ClientAddressDto> conflicts)
                = await _svc.AddressValidateAsync(dto, oldKey: null, ct);

            return Ok(new { ok, message, conflicts });
        }

        // POST: api/clients/{clientId}/addresses
        // body: ClientAddressDto
        [HttpPost]
        public async Task<IActionResult> Create(
            int clientId,
            [FromBody] ClientAddressDto dto,
            CancellationToken ct)
        {
            dto.ClientId = clientId;

            (bool ok, string? message, System.Collections.Generic.IReadOnlyList<ClientAddressDto> conflicts)
                = await _svc.AddressValidateAsync(dto, oldKey: null, ct);

            if (!ok)
            {
                return BadRequest(new { ok, message, conflicts });
            }

            var saved = await _svc.AddressCreateAsync(dto, ct);
            return saved ? Ok(new { ok = true }) : StatusCode(500, new { ok = false, message = "Create failed." });
        }

        // PUT: api/clients/{clientId}/addresses/{addressType}?start=...
        // body: ClientAddressDto
        [HttpPut("{addressType:int}")]
        public async Task<IActionResult> Update(
            int clientId,
            int addressType,
            [FromQuery] DateTime start,
            [FromBody] ClientAddressDto dto,
            CancellationToken ct)
        {
            dto.ClientId = clientId;
            dto.AddressType = addressType;

            // oldKey = คีย์เดิมสำหรับตรวจ conflict
            var oldKey = (clientId: clientId, addressType: addressType, startDate: start);

            (bool ok, string? message, System.Collections.Generic.IReadOnlyList<ClientAddressDto> conflicts)
                = await _svc.AddressValidateAsync(dto, oldKey, ct);

            if (!ok)
            {
                return BadRequest(new { ok, message, conflicts });
            }

            var done = await _svc.AddressUpdateAsync(clientId, addressType, start, dto, ct);
            return done ? Ok(new { ok = true }) : StatusCode(500, new { ok = false, message = "Update failed." });
        }

        // DELETE: api/clients/{clientId}/addresses/{addressType}?start=...&end=...
        [HttpDelete("{addressType:int}")]
        public async Task<IActionResult> Delete(
            int clientId,
            int addressType,
            [FromQuery] DateTime start,
            [FromQuery] DateTime? end,
            CancellationToken ct)
        {
            var done = await _svc.AddressDeleteAsync(clientId, addressType, start, end, ct);
            return done ? Ok(new { ok = true }) : StatusCode(500, new { ok = false, message = "Delete failed." });
        }
    }
}
