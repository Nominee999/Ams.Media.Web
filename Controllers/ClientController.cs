using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ams.Media.Web.Services;
using Ams.Media.Web.Dto;

namespace Ams.Media.Web.Controllers
{
    public class ClientController : Controller
    {
        private readonly IClientService _svc;

        public ClientController(IClientService svc)
        {
            _svc = svc;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? from = "", string? to = "", string? show = null)
        {
            var cookieShow = Request.Cookies.TryGetValue("ShowData", out var c) ? c : null;
            var effectiveShow = (show ?? cookieShow ?? "C").ToUpperInvariant();
            if (effectiveShow != "A" && effectiveShow != "C") effectiveShow = "C";

            var list = await _svc.SearchAsync(from ?? "", to ?? "", effectiveShow);
            return View(list);
        }

        [HttpGet]
        public IActionResult App() => View();

        [HttpGet]
        public IActionResult New()
        {
            var empty = new ClientRow
            {
                ClientId = 0,
                Description = "",
                AgencyCom = 0m,
                ClientPrefix = "",
                CreditTerm = 0,
                IsInUse = false,
                IsNotUse = false
            };
            return View("Edit", empty);
        }

        // ★★★ ค้นหา DB ตรงๆ จาก 4 ฟิลด์ โดยรับ q จาก UI ★★★
        [HttpGet]
        public async Task<IActionResult> Grid(string? q = null, string? show = null)
        {
            try
            {
                var cookieShow = Request.Cookies.TryGetValue("ShowData", out var c) ? c : null;
                var effectiveShow = (show ?? cookieShow ?? "C").ToUpperInvariant();
                if (effectiveShow != "A" && effectiveShow != "C") effectiveShow = "C";

                var list = await _svc.SearchFreeAsync(q, effectiveShow);
                return PartialView("~/Views/Client/_Grid.cshtml", list);
            }
            catch (Exception ex)
            {
                var html = $@"<div class=""alert alert-danger small mb-0"">
    <div class=""fw-semibold"">Load grid failed</div>
    <div class=""text-break"">{System.Net.WebUtility.HtmlEncode(ex.Message)}</div>
</div>";
                return Content(html, "text/html; charset=utf-8");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var row = await _svc.GetAsync(id);
            if (row is null) return NotFound();
            return View(row);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save([FromForm] ClientRow model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (model.ClientId <= 0)
                model.ClientId = await _svc.CreateAsync(model);
            else
                await _svc.UpdateAsync(model);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(long id)
        {
            var row = await _svc.GetAsync(id);
            if (row is null) return NotFound();
            return View(row);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var (ok, msg) = await _svc.DeleteAsync(id);
            if (!ok && !string.IsNullOrWhiteSpace(msg))
            {
                ModelState.AddModelError(string.Empty, msg);
                var row = await _svc.GetAsync(id);
                return View("Delete", row);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Address(long id)
        {
            var row = await _svc.GetAsync(id);
            if (row is null) return NotFound();
            return View(row);
        }
    }
}
