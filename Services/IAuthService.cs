using System.Security.Claims;
using System.Threading.Tasks;

namespace Ams.Media.Web.Services
{
    public interface IAuthService
    {
        /// <summary>
        /// ��Ǩ�ͺ���ʼ�ҹ + ��ԡ� Single-Login (Security_Log)
        /// </summary>
        Task<(bool ok, string message, ClaimsPrincipal? principal)>
            ValidateAsync(string username, string password);

        /// <summary>
        /// ź session �Ѩ�غѹ�ͧ������͡�ҡ Security_Log (��͹ Logout)
        /// </summary>
        Task LogoutLogAsync(string username);
    }
}
