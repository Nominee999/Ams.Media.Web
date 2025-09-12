using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ams.Media.Web.Services
{
    public interface IMenuGate
    {
        /// <summary>
        /// �׹�����¡�����: M/T/R/E/S ����Է���ͧ�����
        /// </summary>
        Task<Dictionary<string, IReadOnlyList<MenuItemVm>>> GetAllAsync(ClaimsPrincipal user);
    }
}
