using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Dapper;

using Ams.Media.Web.Repositories.Interfaces;
using Ams.Media.Web.Dto;

namespace Ams.Media.Web.Controllers
{
    [Authorize]
    public sealed class ClientController : Controller
    {
        private readonly IClientRepository _repo;
        private readonly IClientAddressRepository _addrRepo;
        private readonly string _connStr;

        public ClientController(IClientRepository repo, IClientAddressRepository addrRepo, IConfiguration cfg)
        {
            _repo = repo;
            _addrRepo = addrRepo;
            _connStr = cfg.GetConnectionString("AmsDb")
                    ?? cfg.GetConnectionString("Default")
                    ?? "";
        }

        // ===== Helpers =====
        private async Task<List<SetupTypeDto>> LoadAllowedAddressTypesAsync(CancellationToken ct)
        {
            const string sql = @"
SELECT SetupTypeCode, SetupTypeName
FROM SetupType
WHERE SetupTypeCode IN (1,2,3,4,5)
ORDER BY SetupTypeCode;";
            await using var cn = new SqlConnection(_connStr);
            var rows = await cn.QueryAsync<SetupTypeDto>(new CommandDefinition(sql, cancellationToken: ct));
            return rows.AsList();
        }

        // Next ClientId = MAX(ClientId)+1 (เริ่ม 100000 ถ้ายังไม่มีข้อมูล)
        private async Task<int> GetNextClientIdAsync(CancellationToken ct)
        {
            const string sql = @"SELECT ISNULL(MAX(ClientId), 99999) + 1 FROM Client (NOLOCK);";
            await using var cn = new SqlConnection(_connStr);
            return await cn.ExecuteScalarAsync<int>(new CommandDefinition(sql, cancellationToken: ct));
        }

        // ======================= Client Master (App/Grid) =======================
        [HttpGet]
        public async Task<IActionResult> App(string? q, int page = 1, int pageSize = 50, string show = "A", CancellationToken ct = default)
        {
            var grid = await _repo.ListPagedAsync(q, page, pageSize, show, ct);
            ViewBag.Query = q ?? "";
            ViewBag.Show = string.IsNullOrWhiteSpace(show) ? "A" : show;
            ViewBag.PageSize = pageSize;

            // เปิดโมดัลอัตโนมัติเมื่อมาจาก /Client/New หรือ /Client/Edit/{id}
            ViewBag.Mode = Request.Query["mode"].ToString();          // "new" หรือ ""
            if (int.TryParse(Request.Query["editId"], out var eid))
                ViewBag.EditId = eid;

            return View(grid);
        }

        [HttpGet]
        public async Task<IActionResult> Grid(string? q, int page = 1, int pageSize = 50, string show = "A", CancellationToken ct = default)
        {
            var grid = await _repo.ListPagedAsync(q, page, pageSize, show, ct);
            ViewBag.Query = q ?? "";
            ViewBag.Show = string.IsNullOrWhiteSpace(show) ? "A" : show;
            ViewBag.PageSize = pageSize;
            return PartialView("_Grid", grid);
        }

        // ===== NEW/EDIT Friendly Routes (รองรับลิงก์เดิม) =====
        [HttpGet]
        public IActionResult New() => RedirectToAction(nameof(App), new { mode = "new" });

        [HttpGet]
        public IActionResult Edit(long id) => RedirectToAction(nameof(App), new { editId = id });

        // ===== NEW/EDIT FORM (โหลดเข้าโมดัล) ใช้ View: Views/Client/Edit.cshtml =====
        [HttpGet]
        public async Task<IActionResult> NewForm(CancellationToken ct = default)
        {
            var nextId = await GetNextClientIdAsync(ct);
            var vm = new ClientRow
            {
                ClientId = nextId,                // โชว์เลขถัดไป
                Description = "",
                ClientPrefix = "",
                ClientBranch = "สำนักงานใหญ่",
                AgencyCom = 0.000m,
                CreditTerm = 30,
                ClientTaxNo = "",
                ClientStatus = 0,
                BranchType = 0
            };
            ViewBag.InModal = true;
            ViewBag.IsNew = true;                   // ให้ View โพสต์ไป Create
            return PartialView("Edit", vm);
        }

        [HttpGet]
        public async Task<IActionResult> EditForm(long id, CancellationToken ct = default)
        {
            var row = await _repo.GetAsync(id);
            if (row is null) return NotFound();

            var vm = new ClientRow
            {
                ClientId = row.ClientId,
                Description = row.Description,
                ClientPrefix = row.ClientPrefix,
                ClientBranch = row.ClientBranch,
                AgencyCom = row.AgencyCom,
                CreditTerm = row.CreditTerm,
                ClientTaxNo = row.ClientTaxNo,
                ClientStatus = row.ClientStatus,
                BranchType = row.BranchType
            };
            ViewBag.InModal = true;
            ViewBag.IsNew = false;                  // ให้ View โพสต์ไป Update
            return PartialView("Edit", vm);
        }

        // ===== Create / Update =====
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClientRow dto, CancellationToken ct = default)
        {
            // sanitize
            dto.ClientPrefix = (dto.ClientPrefix ?? "").Trim().ToUpperInvariant();
            dto.Description = (dto.Description ?? "").Trim();
            dto.ClientBranch = string.IsNullOrWhiteSpace(dto.ClientBranch) ? "สำนักงานใหญ่" : dto.ClientBranch.Trim();

            // validate
            if (!Regex.IsMatch(dto.ClientPrefix, @"^[A-Z0-9]{2,4}$"))
            {
                TempData["ClientError"] = "Client Prefix must be 2–4 characters [A–Z0–9] with no spaces.";
                return RedirectToAction(nameof(App));
            }
            if (dto.Description.Length == 0 || dto.Description.Length > 50)
            {
                TempData["ClientError"] = "Client Name is required and must be ≤ 50 characters.";
                return RedirectToAction(nameof(App));
            }
            if (dto.AgencyCom < 0m || dto.AgencyCom > 100m)
            {
                TempData["ClientError"] = "Agency Commission must be between 0 and 100.";
                return RedirectToAction(nameof(App));
            }
            if (dto.CreditTerm < 0 || dto.CreditTerm > 365)
            {
                TempData["ClientError"] = "Credit Term must be between 0 and 365 days.";
                return RedirectToAction(nameof(App));
            }
            if (!string.IsNullOrWhiteSpace(dto.ClientTaxNo))
            {
                var tax = dto.ClientTaxNo.Trim();
                if (!Regex.IsMatch(tax, @"^\d{13}$"))
                {
                    TempData["ClientError"] = "Tax ID must be 13 digits or left blank.";
                    return RedirectToAction(nameof(App));
                }
                dto.ClientTaxNo = tax;
            }

            // ยืนยัน ClientId ฝั่งเซิร์ฟเวอร์เสมอ
            if (dto.ClientId <= 0)
                dto.ClientId = await GetNextClientIdAsync(ct);

            // 1) พยายามผ่าน Repository ก่อน
            var ok = await _repo.CreateAsync(dto);
            if (!ok)
            {
                // 2) Fallback: เขียนผ่าน Dapper โดยตรง
                const string sql = @"
INSERT INTO Client
(ClientId, Description, ClientPrefix, ClientBranch, AgencyCom, CreditTerm, ClientTaxNo, ClientStatus, BranchType)
VALUES (@ClientId, @Description, @ClientPrefix, @ClientBranch, @AgencyCom, @CreditTerm, @ClientTaxNo, @ClientStatus, @BranchType);";
                try
                {
                    await using var cn = new SqlConnection(_connStr);
                    var rows = await cn.ExecuteAsync(new CommandDefinition(sql, dto, cancellationToken: ct));
                    ok = rows > 0;
                }
                catch (Exception ex)
                {
                    TempData["ClientError"] = "Create failed: " + ex.Message;
                    return RedirectToAction(nameof(App));
                }
            }

            if (!ok) TempData["ClientError"] = "Create failed.";
            return RedirectToAction(nameof(App));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ClientRow dto, CancellationToken ct = default)
        {
            // sanitize
            dto.ClientPrefix = (dto.ClientPrefix ?? "").Trim().ToUpperInvariant();
            dto.Description = (dto.Description ?? "").Trim();
            dto.ClientBranch = string.IsNullOrWhiteSpace(dto.ClientBranch) ? "สำนักงานใหญ่" : dto.ClientBranch.Trim();

            // validate
            if (!Regex.IsMatch(dto.ClientPrefix, @"^[A-Z0-9]{2,4}$"))
            {
                TempData["ClientError"] = "Client Prefix must be 2–4 characters [A–Z0–9] with no spaces.";
                return RedirectToAction(nameof(App));
            }
            if (dto.Description.Length == 0 || dto.Description.Length > 50)
            {
                TempData["ClientError"] = "Client Name is required and must be ≤ 50 characters.";
                return RedirectToAction(nameof(App));
            }
            if (dto.AgencyCom < 0m || dto.AgencyCom > 100m)
            {
                TempData["ClientError"] = "Agency Commission must be between 0 and 100.";
                return RedirectToAction(nameof(App));
            }
            if (dto.CreditTerm < 0 || dto.CreditTerm > 365)
            {
                TempData["ClientError"] = "Credit Term must be between 0 and 365 days.";
                return RedirectToAction(nameof(App));
            }
            if (!string.IsNullOrWhiteSpace(dto.ClientTaxNo))
            {
                var tax = dto.ClientTaxNo.Trim();
                if (!Regex.IsMatch(tax, @"^\d{13}$"))
                {
                    TempData["ClientError"] = "Tax ID must be 13 digits or left blank.";
                    return RedirectToAction(nameof(App));
                }
                dto.ClientTaxNo = tax;
            }

            // 1) พยายามผ่าน Repository ก่อน
            var ok = await _repo.UpdateAsync(dto);
            if (!ok)
            {
                // 2) Fallback: อัปเดตผ่าน Dapper ตรงๆ
                const string sql = @"
UPDATE Client SET
    Description  = @Description,
    ClientPrefix = @ClientPrefix,
    ClientBranch = @ClientBranch,
    AgencyCom    = @AgencyCom,
    CreditTerm   = @CreditTerm,
    ClientTaxNo  = @ClientTaxNo,
    ClientStatus = @ClientStatus,
    BranchType   = @BranchType
WHERE ClientId = @ClientId;";
                try
                {
                    await using var cn = new SqlConnection(_connStr);
                    var rows = await cn.ExecuteAsync(new CommandDefinition(sql, dto, cancellationToken: ct));
                    ok = rows > 0;
                }
                catch (Exception ex)
                {
                    TempData["ClientError"] = "Update failed: " + ex.Message;
                    return RedirectToAction(nameof(App));
                }
            }

            if (!ok) TempData["ClientError"] = "Update failed.";
            return RedirectToAction(nameof(App), new { editId = dto.ClientId });
        }

        // ===== Delete Client (Soft/Hard ตามรีโพ) พร้อม Fallback =====
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id, CancellationToken ct = default)
        {
            var ok = await _repo.DeleteAsync(id);
            if (!ok)
            {
                // Fallback: ลอง soft-delete ด้วยการเซ็ตสถานะ 0
                const string sql = @"UPDATE Client SET ClientStatus = 0 WHERE ClientId = @id;";
                try
                {
                    await using var cn = new SqlConnection(_connStr);
                    var rows = await cn.ExecuteAsync(new CommandDefinition(sql, new { id }, cancellationToken: ct));
                    ok = rows > 0;
                }
                catch (Exception ex)
                {
                    TempData["ClientError"] = "Delete failed: " + ex.Message;
                    return RedirectToAction(nameof(App));
                }
            }

            if (!ok) TempData["ClientError"] = "Cannot delete client (in use or not found).";
            return RedirectToAction(nameof(App));
        }

        // (รองรับลิงก์เดิมที่อาจเรียกเป็น GET)
        [HttpGet]
        public async Task<IActionResult> Delete(long id)
        {
            var ok = await _repo.DeleteAsync(id);
            if (!ok)
            {
                const string sql = @"UPDATE Client SET ClientStatus = 0 WHERE ClientId = @id;";
                try
                {
                    await using var cn = new SqlConnection(_connStr);
                    var rows = await cn.ExecuteAsync(sql, new { id });
                    ok = rows > 0;
                }
                catch (Exception ex)
                {
                    TempData["ClientError"] = "Delete failed: " + ex.Message;
                    return RedirectToAction(nameof(App));
                }
            }

            if (!ok) TempData["ClientError"] = "Cannot delete client (in use or not found).";
            return RedirectToAction(nameof(App));
        }

        // ======================= Address (ของเดิม) =======================
        [HttpGet]
        public async Task<IActionResult> Address(long id, CancellationToken ct = default)
        {
            var client = await _repo.GetAsync(id);
            if (client is null) return NotFound();

            var types = await LoadAllowedAddressTypesAsync(ct);
            var allowedCodes = new HashSet<int>(types.Select(t => t.SetupTypeCode));
            var typeMap = types.ToDictionary(t => t.SetupTypeCode, t => t.SetupTypeName);

            var listAll = await _addrRepo.ListAsync((int)id, null, ct);
            var list = listAll.Where(a => allowedCodes.Contains(a.AddressType))
                              .OrderBy(a => a.AddressType)
                              .ThenBy(a => a.StartDate)
                              .ToList();

            ViewBag.Client = client;
            ViewBag.AddressTypes = types;
            ViewBag.AddressTypeMap = typeMap;
            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> AddressEdit(long clientId, int type, string start, CancellationToken ct = default)
        {
            if (!DateTime.TryParseExact(start, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var startDate))
                return BadRequest("Invalid start date.");

            var client = await _repo.GetAsync(clientId);
            if (client is null) return NotFound();

            var types = await LoadAllowedAddressTypesAsync(ct);
            var allowedCodes = new HashSet<int>(types.Select(t => t.SetupTypeCode));
            if (!allowedCodes.Contains(type)) return BadRequest("Invalid address type.");

            var one = await _addrRepo.GetAsync((int)clientId, type, startDate, null, ct);
            if (one is null) return NotFound();

            ViewBag.Client = client;
            ViewBag.AddressTypes = types;
            ViewBag.AddressTypeMap = types.ToDictionary(t => t.SetupTypeCode, t => t.SetupTypeName);

            return View("Address", new[] { one });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddressCreate(long clientId, ClientAddressDto input, CancellationToken ct = default)
        {
            input.ClientId = (int)clientId;

            var types = await LoadAllowedAddressTypesAsync(ct);
            var allowedCodes = new HashSet<int>(types.Select(t => t.SetupTypeCode));
            if (!allowedCodes.Contains(input.AddressType))
            {
                TempData["AddrError"] = "Invalid address type.";
                return RedirectToAction(nameof(Address), new { id = clientId });
            }

            if (input.EndDate.HasValue && input.StartDate > input.EndDate.Value)
            {
                TempData["AddrError"] = "StartDate must be <= EndDate.";
                return RedirectToAction(nameof(Address), new { id = clientId });
            }

            var ok = await _addrRepo.CreateAsync(input, ct);
            if (!ok) TempData["AddrError"] = "Cannot create address (overlap or invalid).";

            return RedirectToAction(nameof(Address), new { id = clientId });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddressUpdate(long clientId, int addressType, string start, ClientAddressDto input, CancellationToken ct = default)
        {
            if (!DateTime.TryParseExact(start, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var startDate))
                return BadRequest("Invalid start date.");

            var types = await LoadAllowedAddressTypesAsync(ct);
            var allowedCodes = new HashSet<int>(types.Select(t => t.SetupTypeCode));
            if (!allowedCodes.Contains(addressType))
            {
                TempData["AddrError"] = "Invalid address type.";
                return RedirectToAction(nameof(Address), new { id = clientId });
            }

            input.ClientId = (int)clientId;
            if (input.AddressType == 0) input.AddressType = addressType;

            if (input.EndDate.HasValue && input.StartDate > input.EndDate.Value)
            {
                TempData["AddrError"] = "StartDate must be <= EndDate.";
                return RedirectToAction(nameof(Address), new { id = clientId });
            }

            var ok = await _addrRepo.UpdateAsync((int)clientId, addressType, startDate, input, ct);
            if (!ok) TempData["AddrError"] = "Cannot update address (overlap or invalid).";

            return RedirectToAction(nameof(Address), new { id = clientId });
        }

        [HttpGet]
        public async Task<IActionResult> AddressDelete(long clientId, int type, string start, string? end, CancellationToken ct = default)
        {
            if (!DateTime.TryParseExact(start, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var startDate))
                return BadRequest("Invalid start date.");

            var types = await LoadAllowedAddressTypesAsync(ct);
            var allowedCodes = new HashSet<int>(types.Select(t => t.SetupTypeCode));
            if (!allowedCodes.Contains(type))
            {
                TempData["AddrError"] = "Invalid address type.";
                return RedirectToAction(nameof(Address), new { id = clientId });
            }

            DateTime? endDate = null;
            if (!string.IsNullOrWhiteSpace(end))
            {
                if (!DateTime.TryParseExact(end, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var e))
                    return BadRequest("Invalid end date.");
                endDate = e;
            }

            var ok = await _addrRepo.DeleteAsync((int)clientId, type, startDate, endDate, ct);
            if (!ok) TempData["AddrError"] = "Cannot delete address (in use or not found).";

            return RedirectToAction(nameof(Address), new { id = clientId });
        }
    }
}
