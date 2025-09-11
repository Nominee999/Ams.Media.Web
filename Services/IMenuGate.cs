using System.Security.Claims;
using System.Threading.Tasks;
using Ams.Media.Web.Models;

namespace Ams.Media.Web.Services
{
    public interface IMenuGate
    {
        Task<IReadOnlyList<MenuItemVm>> GetSubMenusAsync(string groupKey, ClaimsPrincipal user);
    }
}
