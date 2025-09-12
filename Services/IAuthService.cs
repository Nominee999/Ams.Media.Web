using System.Security.Claims;
using System.Threading.Tasks;

namespace Ams.Media.Web.Services
{
    public interface IAuthService
    {
        /// <summary>
        /// ตรวจสอบรหัสผ่าน + กติกา Single-Login (Security_Log)
        /// </summary>
        Task<(bool ok, string message, ClaimsPrincipal? principal)>
            ValidateAsync(string username, string password);

        /// <summary>
        /// ลบ session ปัจจุบันของผู้ใช้ออกจาก Security_Log (ใช้ตอน Logout)
        /// </summary>
        Task LogoutLogAsync(string username);
    }
}
