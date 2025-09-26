using Ams.Media.Web.Models;
using Ams.Media.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ams.Media.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _auth;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAuthService auth, ILogger<AccountController> logger)
        {
            _auth = auth;
            _logger = logger;
        }

        [HttpGet, AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginVm());
        }

        // ★ ให้เหลือ POST แค่อันนี้
        [HttpPost, AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVm model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ReturnUrl"] = returnUrl;
                return View(model);
            }

            var (ok, message, principal) =
                await _auth.ValidateAsync((model.UserName ?? "").Trim(), (model.Password ?? "").Trim());

            if (!ok || principal == null)
            {
                ModelState.AddModelError(string.Empty, message);
                ViewData["ReturnUrl"] = returnUrl;
                return View(model);
            }

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }



        // ✅ รองรับ GET เพื่อให้ลิงก์ <a href="/Account/Logout"> ทำงานได้ (ชั่วคราว)
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            var uname = User?.Identity?.Name?.Trim();
            try
            {
                if (!string.IsNullOrEmpty(uname))
                    await _auth.LogoutLogAsync(uname);
            }
            catch { /* swallow เพื่อไม่ให้ Logout พัง */ }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        // ✅ เวอร์ชัน POST (ทางการ ปลอดภัยกว่า)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogoutPost()
        {
            var uname = User?.Identity?.Name?.Trim();
            try
            {
                if (!string.IsNullOrEmpty(uname))
                    await _auth.LogoutLogAsync(uname);
            }
            catch { /* swallow */ }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        [HttpGet, AllowAnonymous]
        public IActionResult Denied() => View();
    }
}
