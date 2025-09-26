// ===== FILE: Services/MenuGate.cs =====
using System.Reflection;
using Ams.Media.Web.Data;
using Ams.Media.Web.Models; // ต้องมี MenuItemVm และ SecurityMenu
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

        /// <summary>
        /// ดึงเมนูย่อยตามกลุ่มตัวอักษร (M/T/R/E/A/S) ของผู้ใช้ที่ระบุ
        /// - อ่านสิทธิ์จาก Security_Menu (1 = มีสิทธิ์)
        /// - ใช้ reflection ไล่พร็อพที่ขึ้นต้นด้วยตัวอักษรกลุ่ม (เช่น 'm' สำหรับ Master)
        /// - แมป key -> (Text, Url, Icon) ด้วย MapMenuKey
        /// </summary>
        public async Task<IReadOnlyList<MenuItemVm>> GetSubMenusAsync(char group, string username)
        {
            var g = char.ToUpperInvariant(group);
            if ("MTREAS".IndexOf(g) < 0) return Array.Empty<MenuItemVm>();

            var uname = (username ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(uname)) return Array.Empty<MenuItemVm>();

            var row = await _db.SecurityMenus
                .AsNoTracking()
                .FirstOrDefaultAsync(x => (x.Username ?? string.Empty).Trim() == uname);

            if (row is null) return Array.Empty<MenuItemVm>();

            var prefixLower = char.ToLowerInvariant(g); // 'm' / 't' / ...
            var props = typeof(SecurityMenu)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p =>
                    p.PropertyType == typeof(string) &&
                    p.Name.Length >= 2 &&
                    char.ToLowerInvariant(p.Name[0]) == prefixLower);

            var items = new List<MenuItemVm>();

            foreach (var prop in props)
            {
                var raw = prop.GetValue(row) as string;
                if ((raw ?? "0").Trim() != "1") continue; // ไม่มีสิทธิ์

                var key = prop.Name; // เช่น mclient, mproduct, ...
                var (text, url, icon) = MapMenuKey(key, g);

                items.Add(new MenuItemVm
                {
                    // หมายเหตุ: ฟิลด์ใน MenuItemVm ของโปรเจกต์คุณควรมี Text/Url/Icon/Enabled
                    Text = text,
                    Url = url,
                    Icon = icon,
                    Enabled = true   // ให้ผ่านเงื่อนไขใน Layout ที่อาจเช็ค x.Enabled
                });
            }

            return items
                .OrderBy(x => x.Text, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        // ===== Helper: map key -> (Text, Url, Icon) =====
        private static (string Text, string Url, string Icon) MapMenuKey(string key, char group)
        {
            var safeKey = key ?? string.Empty;
            var k = safeKey.Trim().ToLowerInvariant();

            switch (char.ToUpperInvariant(group))
            {
                case 'M':
                    // กลุ่ม Master files — เพิ่ม mapping ได้ตามต้องการ
                    return k switch
                    {
                        "mclient" => ("Client", "/Client", "bi bi-people"),
                        "mproduct" => ("Product", "/Product", "bi bi-box-seam"),
                        "mvendor" => ("Vendor", "/Vendor", "bi bi-building"),
                        _ => (safeKey, "#", "bi bi-folder")
                    };

                // กลุ่มอื่น ๆ ยังไม่กำหนด mapping เฉพาะ → fallback
                default:
                    return (safeKey, "#", "bi bi-folder");
            }
        }
    }
}
