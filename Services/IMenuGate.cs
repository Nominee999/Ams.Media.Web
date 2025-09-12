// D:\VS2022\Ams.Media.Web\Services\IMenuGate.cs
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using Ams.Media.Web.Models;

namespace Ams.Media.Web.Services
{
    public interface IMenuGate
    {
        Task<IReadOnlyList<MenuItemVm>> GetSubMenusAsync(string groupKey, ClaimsPrincipal user);
    }
}
