using Ams.Media.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ams.Media.Web.Controllers
{
    public class UiController : Controller
    {
        private readonly IUiStateService _ui;
        public UiController(IUiStateService ui) { _ui = ui; }

        [ValidateAntiForgeryToken]
        [HttpPost("/ui/toggle-ac")]
        public async Task<IActionResult> ToggleAc([FromForm] string? force = null)
        {
            string mode;
            if (!string.IsNullOrWhiteSpace(force))
                mode = await _ui.SetShowDataAsync(force);
            else
                mode = await _ui.ToggleShowDataAsync();

            // ON = Current, OFF = All (เราใช้ค่า "C"=Current, "A"=All)
            // Partial นี้จะถูกแทนที่ด้วย hx-swap="outerHTML"
            return PartialView("~/Views/Shared/_ToggleAC.cshtml", model: mode);
        }
    }
}
