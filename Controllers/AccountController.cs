using System;
using System.Threading.Tasks;
using Ams.Media.Web.Data;
using Ams.Media.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Ams.Media.Web.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly AmsDbContext _db;
        private readonly IAuthService _auth;
        private readonly ILogger<AccountController> _logger;

        public AccountController(AmsDbContext db, IAuthService auth, ILogger<AccountController> logger)
        { _db = db; _auth = auth; _logger = logger; }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, string? returnUrl = null)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Please enter username and password.");
                ViewData["ReturnUrl"] = returnUrl;
                return View();
            }

            var (ok, message, principal) = await _auth.ValidateAsync(username.Trim(), password);

            if (!ok || principal == null)
            {
                ModelState.AddModelError("", message);
                ViewData["ReturnUrl"] = returnUrl;
                return View();
            }

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            return RedirectToLocal(returnUrl);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var uname = User?.Identity?.Name ?? "";
            try { if (!string.IsNullOrWhiteSpace(uname)) await _auth.LogoutLogAsync(uname); }
            catch (Exception ex) { _logger.LogWarning(ex, "LogoutLogAsync failed for {User}", uname); }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        [HttpGet] public IActionResult Denied() => View();

        private IActionResult RedirectToLocal(string? returnUrl)
            => (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                ? Redirect(returnUrl)
                : RedirectToAction("Index", "Home");
    }
}
