using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using Ams.Media.Web.Models;

namespace Ams.Media.Web.Services
{
    public interface IMenuGate
    {
        // สิทธิ์เมนูหลักจาก Claims (menu:M/T/R/E/A/S)
        bool CanMasterfiles(ClaimsPrincipal user);
        bool CanTransactions(ClaimsPrincipal user);
        bool CanReports(ClaimsPrincipal user);
        bool CanEnquirys(ClaimsPrincipal user);
        bool CanAddins(ClaimsPrincipal user);
        bool CanSystems(ClaimsPrincipal user);

        // โหลดเมนูย่อยตามกลุ่ม: "M","T","R","E","A","S"
        Task<IReadOnlyList<MenuItemVm>> GetSubMenusAsync(string groupKey, ClaimsPrincipal user);
        Task<IReadOnlyList<MenuItemVm>> GetSubMenusAsync(char groupKey, ClaimsPrincipal user);
    }
}
