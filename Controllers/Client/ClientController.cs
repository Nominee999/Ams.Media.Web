using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ams.Media.Web.Controllers
{
    // ทำให้ /Client ชนคอนโทรลเลอร์นี้แน่นอน
    [Route("[controller]")]
    [Authorize] // ถ้าจะเทสต์โดยยังไม่ล็อกอิน ให้คอมเมนต์บรรทัดนี้ แล้วใส่ [AllowAnonymous] ที่แอคชันชั่วคราว
    public class ClientController : Controller
    {
        // GET /Client
        [HttpGet("")]
        // [AllowAnonymous] // ← ใช้ชั่วคราวสำหรับเทสต์ route ถ้ายังไม่ได้ล็อกอิน
        public IActionResult Index()
        {
            ViewData["Title"] = "Client Master";
            return View();
        }
    }
}
