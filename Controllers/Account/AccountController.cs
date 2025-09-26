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

        [Authorize, HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var uname = User?.Identity?.Name ?? "";
            try { if (!string.IsNullOrWhiteSpace(uname)) await _auth.LogoutLogAsync(uname); }
            catch (Exception ex) { _logger.LogWarning(ex, "LogoutLogAsync failed for {User}", uname); }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        [HttpGet, AllowAnonymous]
        public IActionResult Denied() => View();
    }
}
