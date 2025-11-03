// Controllers/ClientController.cs
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ams.Media.Web.Dto;
using Ams.Media.Web.Services;

namespace Ams.Media.Web.Controllers
{
    [Authorize]
    public sealed class ClientController : Controller
    {
        private readonly IClientService _svc;

        public ClientController(IClientService svc)
        {
            _svc = svc;
        }

        // … (โค้ดส่วนอื่นคงเดิม เช่น App, Grid, NewForm, EditForm, Create/Update Client) …

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddressCreate(AddressFormVm vm, CancellationToken ct)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var dto = new ClientAddressDto
            {
                ClientId = vm.ClientId,
                AddressType = vm.AddressType,
                StartDate = vm.StartDate,
                EndDate = vm.EndDate, // null = 9999-12-31 ใน Repo
                AddressTitle = vm.AddressName ?? vm.AddressTitle,
                Address01 = vm.Address01,
                Address02 = vm.Address02,
                Address03 = vm.Address03,
                Address04 = vm.Address04
            };

            var ok = await _svc.AddressCreateAsync(dto, ct);
            if (!ok) return Conflict(new { message = "Address date range overlaps" });

            return RedirectToAction("Address", new { id = vm.ClientId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddressUpdate(AddressFormVm vm, CancellationToken ct)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (vm.OldStart == null)
                return BadRequest(new { message = "OldStart is required" });

            var dto = new ClientAddressDto
            {
                ClientId = vm.ClientId,
                AddressType = vm.AddressType,
                StartDate = vm.StartDate,
                EndDate = vm.EndDate,
                AddressTitle = vm.AddressName ?? vm.AddressTitle,
                Address01 = vm.Address01,
                Address02 = vm.Address02,
                Address03 = vm.Address03,
                Address04 = vm.Address04
            };

            var ok = await _svc.AddressUpdateAsync(vm.ClientId, vm.AddressType, vm.OldStart.Value, dto, ct);
            if (!ok) return Conflict(new { message = "Address date range overlaps" });

            return RedirectToAction("Address", new { id = vm.ClientId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddressDelete(AddressDeleteVm vm, CancellationToken ct)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var ok = await _svc.AddressDeleteAsync(vm.ClientId, vm.AddressType, vm.StartDate, vm.EndDate, ct);
            if (!ok) return BadRequest(new { message = "Delete failed" });
            return RedirectToAction("Address", new { id = vm.ClientId });
        }

        public sealed class AddressFormVm
        {
            public int ClientId { get; set; }
            public int AddressType { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public DateTime? OldStart { get; set; }

            public string? AddressTitle { get; set; }
            public string? AddressName { get; set; }
            public string? Address01 { get; set; }
            public string? Address02 { get; set; }
            public string? Address03 { get; set; }
            public string? Address04 { get; set; }
        }

        public sealed class AddressDeleteVm
        {
            public int ClientId { get; set; }
            public int AddressType { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime? EndDate { get; set; }
        }
    }
}
