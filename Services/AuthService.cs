// ===== FILE: Services/AuthService.cs =====

using System.Security.Claims;
using Ams.Media.Web.Data;
using Ams.Media.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Ams.Media.Web.Services
{
    public class AuthService : IAuthService
    {
        private readonly AmsDbContext _db;
        private readonly IConfiguration _config;

        public AuthService(AmsDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public async Task<(bool ok, string message, ClaimsPrincipal? principal)>
            ValidateAsync(string username, string password)
        {
            var uname = (username ?? "").Trim();
            var pwd = (password ?? "").Trim();

            // --- ตรวจผู้ใช้ + สถานะอนุมัติ ---
            var u = await _db.SecurityUsers.AsNoTracking()
                                           .FirstOrDefaultAsync(x => (x.Username ?? "").Trim() == uname);
            if (u == null) return (false, "User not found.", null);
            if (((u.Approved ?? "0").Trim() != "1"))
                return (false, "User is not approved.", null);

            // --- ตรวจรหัสผ่าน: Plain หรือ BCrypt ตาม config ---
            var mode = _config.GetValue<string>("Auth:PasswordMode") ?? "Plain";
            bool passOk = mode.Equals("Plain", StringComparison.OrdinalIgnoreCase)
                ? string.Equals((u.Password ?? "").Trim(), pwd)
                : BCrypt.Net.BCrypt.Verify(pwd, u.Password ?? string.Empty);
            if (!passOk) return (false, "Invalid password.", null);

            // --- Single-Login + MAXUSER ---
            var comp = (Environment.MachineName ?? "UNKNOWN").Trim();
            var now = DateTime.Now;

            // 1) หาแถวของ user ก่อนเสมอ (กันชน MAXUSER หากเป็นผู้ใช้ที่นับอยู่แล้ว)
            var existing = await _db.SecurityLogs.FirstOrDefaultAsync(x => (x.Username ?? "").Trim() == uname);

            if (existing != null)
            {
                // 1.1) เครื่องเดียวกัน => refresh เวลา แล้วผ่าน
                if (string.Equals((existing.ComputerName ?? "").Trim(), comp, StringComparison.OrdinalIgnoreCase))
                {
                    existing.UserDateTime = now;
                    existing.Processing = "Login refreshed";
                    await _db.SaveChangesAsync();
                }
                else
                {
                    // 1.2) คนละเครื่อง => ปฏิเสธตามกติกา single-login
                    string holdComp = (existing.ComputerName ?? "UNKNOWN").Trim();
                    string holdTime = existing.UserDateTime?.ToString("dd/MM/yyyy HH:mm:ss") ?? "-";
                    return (false, $"Login denied. Already logged in at \"{holdComp}\" since {holdTime}.", null);
                }
            }
            else
            {
                // 2) ผู้ใช้ใหม่กำลังจะเพิ่ม active +1 -> เช็ค MAXUSER ที่นี่เท่านั้น
                var enableMax = _config.GetValue<bool?>("AMS:EnableMaxUser") ?? true;
                if (enableMax)
                {
                    var maxUser = _config.GetValue<int?>("AMS:MaxUser") ?? 8;

                    // นับ active ปัจจุบัน (1 แถว = 1 ผู้ใช้ที่ออนไลน์)
                    var active = await _db.SecurityLogs.CountAsync();

                    if (active >= maxUser)
                    {
                        return (false, $"System full: {active}/{maxUser} users online. Please try later.", null);
                    }
                }

                // ผ่าน => insert แถวใหม่
                _db.SecurityLogs.Add(new SecurityLog
                {
                    Username = uname,
                    ComputerName = comp,
                    UserDateTime = now,
                    Processing = "Login OK"
                });
                await _db.SaveChangesAsync();
            }

            // --- ออก ClaimsPrincipal (เหมือนเดิม) ---
            var claims = new List<Claim> { new(ClaimTypes.Name, u.Username ?? string.Empty) };
            return (true, "OK", new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies")));
        }

        public async Task LogoutLogAsync(string username)
        {
            var uname = (username ?? "").Trim();
            var row = await _db.SecurityLogs.FirstOrDefaultAsync(x => (x.Username ?? "").Trim() == uname);
            if (row != null)
            {
                _db.SecurityLogs.Remove(row);
                await _db.SaveChangesAsync();
            }
        }
    }
}
