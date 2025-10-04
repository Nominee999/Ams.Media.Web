using System;
using Microsoft.AspNetCore.Mvc;

namespace Ams.Media.Web.Controllers
{
    public class FlagsController : Controller
    {
        // ---------- A/C (Current / All) ----------
        [ValidateAntiForgeryToken]
        [HttpPost("/flags/showdata/toggle")]
        public IActionResult ToggleShowData()
        {
            var cur = Request.Cookies.TryGetValue("ShowData", out var v) ? (v ?? "C").ToUpperInvariant() : "C";
            var next = (cur == "A") ? "C" : "A";

            Response.Cookies.Append("ShowData", next, new Microsoft.AspNetCore.Http.CookieOptions
            {
                HttpOnly = false,
                IsEssential = true,
                SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddYears(1)
            });

            return PartialView("~/Views/Shared/Partials/_ShowDataToggle.cshtml");
        }

        // ---------- THEME (dark / light) ----------
        [ValidateAntiForgeryToken]
        [HttpPost("/flags/theme/toggle")]
        public IActionResult ToggleTheme()
        {
            // อ่านค่าปัจจุบันจาก cookie
            var cur = Request.Cookies.TryGetValue("Theme", out var v) ? (v ?? "dark").ToLowerInvariant() : "dark";
            if (cur != "dark" && cur != "light") cur = "dark";
            var next = (cur == "dark") ? "light" : "dark";

            Response.Cookies.Append("Theme", next, new Microsoft.AspNetCore.Http.CookieOptions
            {
                HttpOnly = false,
                IsEssential = true,
                SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddYears(1)
            });

            // คืนปุ่ม + สคริปต์เล็ก ๆ เพื่อสลับ data-bs-theme ทันทีโดยไม่ต้องรีเฟรชทั้งหน้า
            return PartialView("~/Views/Shared/Partials/_ThemeToggle.cshtml", model: next);
        }
    }
}
