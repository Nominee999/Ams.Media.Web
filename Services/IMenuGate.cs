using System.Collections.Generic;
using System.Threading.Tasks;
using Ams.Media.Web.Models;

namespace Ams.Media.Web.Services
{
    public interface IMenuGate
    {
        // ให้ _TopNav.cshtml เรียกใช้
        Task<IReadOnlyList<MenuItemVm>> GetSubMenusAsync(string topKey);
    }
}
