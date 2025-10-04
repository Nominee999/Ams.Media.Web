// ===== FILE: Services/MenuGate.cs =====
using Ams.Media.Web.Data;
using Ams.Media.Web.Models; // ต้องมี MenuItemVm และ SecurityMenu
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;

namespace Ams.Media.Web.Services
{
    public class MenuGate : IMenuGate
    {
        private readonly AmsDbContext _db;

        public MenuGate(AmsDbContext db)
        {
            _db = db;
        }

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
                var kLower = (key ?? "").Trim().ToLowerInvariant();

                // ✅ สำคัญ: map เมนู Client ให้ไปที่ Controller/Action ที่ถูกต้อง
                if (g == 'M' && kLower == "mclient")
                {
                    items.Add(new MenuItemVm
                    {
                        Text = "Client",
                        Controller = "Client",
                        Action = "App",
                        Icon = "bi bi-people",
                        Enabled = true
                    });
                    continue;
                }

                // กลุ่มอื่น/คีย์อื่น ๆ fallback เป็นลิงก์ทั่วไป
                var (text, url, icon) = MapFallback(key ?? string.Empty, g);

                items.Add(new MenuItemVm
                {
                    Text = text,
                    Url = url,
                    Icon = icon,
                    Enabled = true
                });
            }

            return items
                .OrderBy(x => x.Text, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        // ===== Helper: fallback key -> (Text, Url, Icon) =====
        private static (string Text, string Url, string Icon) MapFallback(string key, char group)
        {
            var safeKey = key ?? string.Empty;
            var k = safeKey.Trim().ToLowerInvariant();

            switch (char.ToUpperInvariant(group))
            {
                case 'M':
                    return k switch
                    {
                        "mproduct" => ("Product", "/Product", "bi bi-box-seam"),
                        "mvendor" => ("Vendor", "/Vendor", "bi bi-building"),
                        _ => (safeKey, "#", "bi bi-folder")
                    };
                default:
                    return (safeKey, "#", "bi bi-folder");
            }
        }
    }
}
