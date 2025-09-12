using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ams.Media.Web.Services
{
    public interface IMenuGate
    {
        /// <summary>
        /// คืนเมนูแยกกลุ่ม: M/T/R/E/S ตามสิทธิ์ของผู้ใช้
        /// </summary>
        Task<Dictionary<string, IReadOnlyList<MenuItemVm>>> GetAllAsync(ClaimsPrincipal user);
    }
}
