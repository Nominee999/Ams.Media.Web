using System.Reflection;
using System.Security.Claims;
using Ams.Media.Web.Data;
using Ams.Media.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Ams.Media.Web.Services
{
    public class MenuGate : IMenuGate
    {
        private readonly AmsDbContext _db;

        public MenuGate(AmsDbContext db)
        {
            _db = db;
        }

        // ===== Permission flags from security_user (fallback ถ้าไม่มีใช้ให้เป็น true เพื่อให้เห็นเมนูอย่างน้อย) =====
        private async Task<SecurityUser?> GetUserAsync(ClaimsPrincipal user)
        {
            var uname = (user?.Identity?.Name ?? "").Trim();
            if (string.IsNullOrEmpty(uname)) return null;
            return await _db.SecurityUsers.AsNoTracking()
                        .FirstOrDefaultAsync(x => (x.Username ?? "").Trim() == uname);
        }

        public bool CanMasterfiles(ClaimsPrincipal user) => true;
        public bool CanTransactions(ClaimsPrincipal user) => true;
        public bool CanReports(ClaimsPrincipal user) => true;
        public bool CanEnquirys(ClaimsPrincipal user) => true;
        public bool CanAddins(ClaimsPrincipal user) => true;
        public bool CanSystems(ClaimsPrincipal user) => true;

        // ===== Public API =====
        public Task<IReadOnlyList<MenuItemVm>> GetSubMenusAsync(string groupKey, ClaimsPrincipal user)
            => GetSubMenusAsync((groupKey ?? "M").FirstOrDefault(), user);

        public async Task<IReadOnlyList<MenuItemVm>> GetSubMenusAsync(char groupKey, ClaimsPrincipal user)
        {
            var uname = (user?.Identity?.Name ?? "").Trim();
            if (string.IsNullOrEmpty(uname)) return Array.Empty<MenuItemVm>();

            // อ่านโพรไฟล์จาก Security_menu
            var sec = await _db.SecurityMenus.AsNoTracking()
                         .FirstOrDefaultAsync(x => (x.Username ?? "").Trim() == uname);
            if (sec == null) return Array.Empty<MenuItemVm>();

            // กลุ่ม -> พร็อพ prefix
            var prefix = char.ToLowerInvariant(groupKey); // m t r e a s
            var props = typeof(SecurityMenu)
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(p => p.PropertyType == typeof(string)
                                 && p.Name.Length >= 2
                                 && char.ToLowerInvariant(p.Name[0]) == prefix)
                        .OrderBy(p => p.Name)
                        .ToList();

            // map ชื่อใช้งานได้สวย ๆ
            string Pretty(string code)
            {
                if (string.IsNullOrWhiteSpace(code)) return code;
                // ตัดตัวแรก (prefix) แล้วเติมช่องว่างตัวพิมพ์ใหญ่
                var t = code.Substring(1);
                var name = new System.Text.StringBuilder();
                foreach (var ch in t)
                {
                    if (char.IsUpper(ch) && name.Length > 0) name.Append(' ');
                    name.Append(ch);
                }
                return name.ToString();
            }

            // สร้างเมนูเฉพาะฟิลด์ที่ค่า == '1'
            var items = new List<MenuItemVm>();
            foreach (var p in props)
            {
                var raw = (string?)p.GetValue(sec) ?? "0";
                var enabled = (raw.Trim() == "1");
                items.Add(new MenuItemVm
                {
                    Code = p.Name,               // เช่น mcurrency
                    Name = Pretty(p.Name),       // เช่น "currency" -> "Currency"
                    Enabled = enabled,
                    // ยังไม่รู้ controller/action ที่แท้จริง → ให้เปิดไป Home/Index ชั่วคราว
                    Controller = enabled ? "Home" : "",
                    Action = enabled ? "Index" : "",
                    Url = "" // ถ้ารู้เส้นจริงค่อยแมปทีหลัง
                });
            }

            return items;
        }
    }
}
