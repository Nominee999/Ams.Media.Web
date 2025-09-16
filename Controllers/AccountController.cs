using System.Threading.Tasks;
using Ams.Media.Web.Data;
using Ams.Media.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ams.Media.Web.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly AmsDbContext _db;          // เผื่อใช้กรณีอื่นในอนาคต
        private readonly IAuthService _auth;
        private readonly ILogger<AccountController> _logger;

        public AccountController(AmsDbContext db, IAuthService auth, ILogger<AccountController> logger)
        {
            _db = db;
            _auth = auth;
            _logger = logger;
        }

        // ===== Login (GET) =====
        // รองรับ ?ReturnUrl=... เสมอ ป้องกัน 400 BadRequest เวลา framework แนบ returnUrl มา
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // ===== Login (POST) =====
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

            var (ok, message, principal) = await _auth.ValidateAsync(username, password);

            if (!ok || principal == null)
            {
                // กรณี Single-Login ปฏิเสธ จะส่งข้อความบอกว่าเข้าใช้งานที่เครื่องอื่นอยู่
                ModelState.AddModelError("", message);
                ViewData["ReturnUrl"] = returnUrl;
                return View();
            }

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            return RedirectToLocal(returnUrl);
        }

        // ===== Logout (POST) =====
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var uname = User?.Identity?.Name ?? "";

            // ลบ session ปัจจุบันออกจาก Security_Log ตามกติกาข้อ 3
            if (!string.IsNullOrWhiteSpace(uname))
            {
                try
                {
                    await _auth.LogoutLogAsync(uname);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "LogoutLogAsync failed for user {User}", uname);
                    // ต่อให้ลบ log ไม่สำเร็จ ก็ต้อง sign-out ผู้ใช้ให้ได้
                }
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        // ===== Access Denied (เผื่อ pipeline เรียก) =====
        [HttpGet]
        public IActionResult Denied()
            => View();

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            // จุดเริ่มต้นหลัง login (ปรับตามต้องการ)
            return RedirectToAction("Index", "Home");
        }
    }
}
