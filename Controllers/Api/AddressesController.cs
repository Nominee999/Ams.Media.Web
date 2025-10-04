using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ams.Media.Web.Dto;
using Ams.Media.Web.Repositories.Interfaces;

namespace Ams.Media.Web.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public sealed class AddressesController : ControllerBase
    {
        private readonly IClientAddressRepository _repo;

        public AddressesController(IClientAddressRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// GET: /api/addresses?clientId=100000&type=1
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> List(
            [FromQuery] int clientId,
            [FromQuery] int? type,
            CancellationToken ct)
        {
            var items = await _repo.ListAsync(clientId, type, ct);
            return Ok(items);
        }

        /// <summary>
        /// POST: /api/addresses
        /// Body: ClientAddressDto
        /// Optional query: oldStart=yyyy-MM-dd&oldEnd=yyyy-MM-dd (เมื่อเป็นการแก้ไข)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Upsert(
            [FromBody] ClientAddressDto dto,
            [FromQuery] DateTime? oldStart,
            [FromQuery] DateTime? oldEnd,
            CancellationToken ct)
        {
            // ตรวจ overlap ฝั่ง DB ควรถูกทำใน service/repo อื่น ๆ แล้ว
            await _repo.UpsertAsync(dto, oldStart, oldEnd, ct);
            return Ok(new { ok = true });
        }

        /// <summary>
        /// DELETE: /api/addresses?clientId=100000&addressType=1&start=2024-01-01&end=2024-12-31
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> Delete(
            [FromQuery] int clientId,
            [FromQuery] int addressType,
            [FromQuery] DateTime start,
            [FromQuery] DateTime end,
            CancellationToken ct)
        {
            var ok = await _repo.DeleteAsync(clientId, addressType, start, end, ct);
            return Ok(new { ok });
        }
    }
}
