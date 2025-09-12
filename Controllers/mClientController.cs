using System.ComponentModel.DataAnnotations;
using Ams.Media.Web.Data;
using Ams.Media.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ams.Media.Web.Controllers
{
    // Masterfiles / Client
    public class mClientController : Controller
    {
        private readonly AmsDbContext _db;
        public mClientController(AmsDbContext db) { _db = db; }

        // GET: /Masterfiles/mClient
        public async Task<IActionResult> Index(string? q)
        {
            var query = _db.Clients.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(c =>
                    (c.ClieName ?? "").Contains(q) ||
                    c.ClientId.ToString().Contains(q));
            }

            var list = await query
                .OrderBy(c => c.ClientId)
                .Select(c => new ClientListVm
                {
                    ClientId = c.ClientId,
                    ClientName = c.ClieName ?? "",
                    IsActive = c.ClientStatus == "1"
                })
                .ToListAsync();

            return View(list);
        }

        // GET: /Masterfiles/mClient/Detail/5
        public async Task<IActionResult> Detail(int id)
        {
            var c = await _db.Clients.AsNoTracking().FirstOrDefaultAsync(x => x.ClientId == id);
            if (c == null) return NotFound();

            var vm = new ClientDetailVm
            {
                ClientId = c.ClientId,
                ClientName = c.ClieName ?? "",
                CreditTerm = c.CreditTerm,
                AgencyCom = c.AgencyCom,
                ClientPrefix = c.ClientPrefix,
                ClientBranch = c.ClientBranch,
                ClientTaxNo = c.ClientTaxNo,
                BranchType = c.BranchType,
                IsActive = c.ClientStatus == "1"
            };
            return View(vm);
        }

        // GET: /Masterfiles/mClient/Create
        public IActionResult Create() => View(new ClientEditVm { IsActive = true });

        // POST: /Masterfiles/mClient/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] ClientEditVm vm)
        {
            if (!ModelState.IsValid) return View(vm);

            // NOTE: ถ้า ClientId เป็น identity ให้ลบการตั้งค่า ClientId ออก
            var entity = new Client
            {
                ClientId = vm.ClientId,
                ClieName = vm.ClientName?.Trim(),
                CreditTerm = vm.CreditTerm,
                AgencyCom = vm.AgencyCom,
                ClientPrefix = vm.ClientPrefix?.Trim(),
                ClientBranch = vm.ClientBranch?.Trim(),
                ClientTaxNo = vm.ClientTaxNo?.Trim(),
                BranchType = vm.BranchType?.Trim(),
                ClientStatus = vm.IsActive ? "1" : "0"
            };

            _db.Clients.Add(entity);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Masterfiles/mClient/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var c = await _db.Clients.FirstOrDefaultAsync(x => x.ClientId == id);
            if (c == null) return NotFound();

            var vm = new ClientEditVm
            {
                ClientId = c.ClientId,
                ClientName = c.ClieName ?? "",
                CreditTerm = c.CreditTerm,
                AgencyCom = c.AgencyCom,
                ClientPrefix = c.ClientPrefix,
                ClientBranch = c.ClientBranch,
                ClientTaxNo = c.ClientTaxNo,
                BranchType = c.BranchType,
                IsActive = c.ClientStatus == "1"
            };
            return View(vm);
        }

        // POST: /Masterfiles/mClient/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] ClientEditVm vm)
        {
            if (id != vm.ClientId) return BadRequest();
            if (!ModelState.IsValid) return View(vm);

            var c = await _db.Clients.FirstOrDefaultAsync(x => x.ClientId == id);
            if (c == null) return NotFound();

            c.ClieName = vm.ClientName?.Trim();
            c.CreditTerm = vm.CreditTerm;
            c.AgencyCom = vm.AgencyCom;
            c.ClientPrefix = vm.ClientPrefix?.Trim();
            c.ClientBranch = vm.ClientBranch?.Trim();
            c.ClientTaxNo = vm.ClientTaxNo?.Trim();
            c.BranchType = vm.BranchType?.Trim();
            c.ClientStatus = vm.IsActive ? "1" : "0";

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Detail), new { id = c.ClientId });
        }

        // POST: /Masterfiles/mClient/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var c = await _db.Clients.FirstOrDefaultAsync(x => x.ClientId == id);
            if (c == null) return NotFound();

            _db.Clients.Remove(c);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: /Masterfiles/mClient/ToggleActive/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var c = await _db.Clients.FirstOrDefaultAsync(x => x.ClientId == id);
            if (c == null) return NotFound();

            c.ClientStatus = (c.ClientStatus == "1") ? "0" : "1";
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Detail), new { id });
        }
    }

    // ========= ViewModels =========

    public class ClientListVm
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; } = "";
        public bool IsActive { get; set; }
    }

    public class ClientDetailVm
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; } = "";
        public int? CreditTerm { get; set; }
        public decimal? AgencyCom { get; set; }
        public string? ClientPrefix { get; set; }
        public string? ClientBranch { get; set; }
        public string? ClientTaxNo { get; set; }
        public string? BranchType { get; set; }
        public bool IsActive { get; set; }
    }

    public class ClientEditVm
    {
        [Required]
        public int ClientId { get; set; }  // ถ้าเป็น identity ให้เอาออกจากฟอร์ม

        [Required, StringLength(200)]
        public string ClientName { get; set; } = "";

        [Range(0, 999)]
        public int? CreditTerm { get; set; }

        [Range(typeof(decimal), "0", "9999999")]
        public decimal? AgencyCom { get; set; }

        [StringLength(50)]
        public string? ClientPrefix { get; set; }

        [StringLength(100)]
        public string? ClientBranch { get; set; }

        [StringLength(50)]
        public string? ClientTaxNo { get; set; }

        [StringLength(50)]
        public string? BranchType { get; set; }

        public bool IsActive { get; set; }
    }
}
