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
            // ตรวจสอบ user ใน Security_user
            var u = await _db.SecurityUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Username == username && x.Approved == "1");

            if (u == null)
                return (false, "User not found or not approved.", null);

            var mode = _config.GetValue<string>("Auth:PasswordMode") ?? "Plain";
            bool passOk = mode.Equals("Plain", StringComparison.OrdinalIgnoreCase)
                ? string.Equals(u.Password, password)
                : BCrypt.Net.BCrypt.Verify(password, u.Password ?? string.Empty);

            if (!passOk)
                return (false, "Invalid password.", null);

            // กติกา Single-Login
            var comp = Environment.MachineName ?? "UNKNOWN";
            var now = DateTime.Now;

            var existing = await _db.SecurityLogs.FirstOrDefaultAsync(x => x.Username == username);

            if (existing != null)
            {
                if (string.Equals(existing.ComputerName?.Trim(), comp, StringComparison.OrdinalIgnoreCase))
                {
                    // เครื่องเดิม → update เวลา
                    existing.UserDateTime = now;
                    existing.Processing = "Login refreshed";
                    await _db.SaveChangesAsync();
                }
                else
                {
                    // เครื่องอื่น → ปฏิเสธ login
                    string holdComp = (existing.ComputerName ?? "UNKNOWN").Trim();
                    string holdTime = existing.UserDateTime.HasValue
                        ? existing.UserDateTime.Value.ToString("dd/MM/yyyy HH:mm:ss")
                        : "-";
                    var msg = $"Login denied. Already logged in at \"{holdComp}\" since {holdTime}. Please logout from that machine first.";
                    return (false, msg, null);
                }
            }
            else
            {
                // ไม่มี record → insert ใหม่
                _db.SecurityLogs.Add(new SecurityLog
                {
                    Username = username,
                    ComputerName = comp,
                    UserDateTime = now,
                    Processing = "Login OK"
                });
                await _db.SaveChangesAsync();
            }

            // Claims สำหรับสิทธิ์เมนู
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, u.Username ?? string.Empty),
                new("menu:M", (u.Masterfiles  == "1").ToString()),
                new("menu:T", (u.Transactions == "1").ToString()),
                new("menu:R", (u.Reports      == "1").ToString()),
                new("menu:E", (u.Enquirys     == "1").ToString()),
                new("menu:S", (u.Systems      == "1").ToString()),
                new("menu:A", (u.Addinss      == "1").ToString()),
            };

            return (true, "OK", new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies")));
        }

        public async Task LogoutLogAsync(string username)
        {
            var row = await _db.SecurityLogs.FirstOrDefaultAsync(x => x.Username == username);
            if (row != null)
            {
                _db.SecurityLogs.Remove(row);
                await _db.SaveChangesAsync();
            }
        }
    }
}
