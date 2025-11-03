// Controllers/Api/AddressesController.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ams.Media.Web.Dto;
using Ams.Media.Web.Services;

namespace Ams.Media.Web.Controllers.Api
{
    [ApiController]
    [Route("api/addresses")]
    [Authorize]
    public sealed class AddressesController : ControllerBase
    {
        private readonly IClientService _svc;

        public AddressesController(IClientService svc)
        {
            _svc = svc;
        }

        // POST /api/addresses/upsert
        [HttpPost("upsert")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert([FromBody] AddressUpsertRequest req, CancellationToken ct)
        {
            if (req == null) return BadRequest();

            var dto = new ClientAddressDto
            {
                ClientId = req.ClientId,
                AddressType = req.AddressType,
                StartDate = req.StartDate,
                EndDate = req.EndDate, // null = เปิดปลาย
                AddressTitle = req.AddressName ?? req.AddressTitle,
                Address01 = req.Address01,
                Address02 = req.Address02,
                Address03 = req.Address03,
                Address04 = req.Address04
            };

            var ok = await _svc.AddressUpsertAsync(dto, req.OldStart, req.OldEnd, ct);
            if (!ok) return Conflict(new { message = "Address date range overlaps" });

            return Ok(new { message = "OK" });
        }

        public sealed class AddressUpsertRequest
        {
            public int ClientId { get; set; }
            public int AddressType { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public DateTime? OldStart { get; set; }
            public DateTime? OldEnd { get; set; }

            public string? AddressTitle { get; set; }
            public string? AddressName { get; set; }
            public string? Address01 { get; set; }
            public string? Address02 { get; set; }
            public string? Address03 { get; set; }
            public string? Address04 { get; set; }
        }
    }
}
