using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Ams.Media.Web.Models;

namespace Ams.Media.Web.Services
{
    public interface IMenuGate
    {
        Task<IReadOnlyList<MenuItemVm>> GetSubMenusAsync(char group, string username);

        // ใช้ตรวจสิทธิ์ก่อนลบ
        Task<bool> CanAccessAsync(ClaimsPrincipal user, string feature, CancellationToken ct);
    }
}
