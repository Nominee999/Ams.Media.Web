using System.Collections.Generic;
using System.Threading.Tasks;
using Ams.Media.Web.Models;

namespace Ams.Media.Web.Services
{
    public interface IMenuGate
    {
        // ��� _TopNav.cshtml ���¡��
        Task<IReadOnlyList<MenuItemVm>> GetSubMenusAsync(string topKey);
    }
}
