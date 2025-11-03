// Controllers/Api/SetupController.cs
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ams.Media.Web.Controllers.Api
{
    [ApiController]
    [Route("api/setup")]
    [Authorize]
    public sealed class SetupController : ControllerBase
    {
        // GET /api/setup/address-types
        [HttpGet("address-types")]
        public async Task<IActionResult> GetAddressTypes(CancellationToken ct)
        {
            // แหล่งจริง: SetupType (Code 1..5) — ที่นี่คืน mock minimal เพื่อให้ UI ไม่พัง
            // *** ถ้าคุณมี repo เรียก DB แล้วอยู่ในโปรเจ็กต์ ให้สลับมาเรียกจริง ***
            await Task.Yield();

            var rows = new List<object>
            {
                new { Code = 1, Name = "Head Office" },
                new { Code = 2, Name = "Billing" },
                new { Code = 3, Name = "Delivery" },
                new { Code = 4, Name = "Tax" },
                new { Code = 5, Name = "Others" },
            };

            return Ok(rows);
        }
    }
}
