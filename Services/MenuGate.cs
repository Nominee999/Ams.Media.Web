#nullable enable
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Ams.Media.Web.Data;
using Ams.Media.Web.Models;

namespace Ams.Media.Web.Services
{
    public class MenuGate : IMenuGate
    {
        private readonly AmsDbContext _db;
        public MenuGate(AmsDbContext db) { _db = db; }

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

            var prefixLower = char.ToLowerInvariant(g);
            var props = typeof(SecurityMenu)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.PropertyType == typeof(string)
                         && p.Name.Length >= 2
                         && char.ToLowerInvariant(p.Name[0]) == prefixLower);

            var items = new List<MenuItemVm>();
            foreach (var prop in props)
            {
                // ป้องกัน null จากค่าในตาราง/พร็อพเพอร์ตี้
                var raw = (prop.GetValue(row) as string) ?? "0";
                if (raw.Trim() != "1") continue;

                var key = (prop.Name ?? string.Empty).Trim().ToLowerInvariant();
                if (g == 'M' && key == "mclient")
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

                items.Add(new MenuItemVm
                {
                    Text = prop.Name ?? string.Empty, // ← แก้จุดเตือน CS8601
                    Url = "#",
                    Icon = "bi bi-folder",
                    Enabled = true
                });
            }
            return items.OrderBy(x => x.Text, System.StringComparer.OrdinalIgnoreCase).ToList();
        }

        public Task<bool> CanAccessAsync(ClaimsPrincipal user, string feature, CancellationToken ct)
            => Task.FromResult(true); // TODO: ใส่กติกาสิทธิ์จริงภายหลัง
    }
}
