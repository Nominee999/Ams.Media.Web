using System.Security.Claims;

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

        // โหลดเมนูย่อยตามกลุ่ม: "M","T","R","E","S"
        Task<IReadOnlyList<MenuItemVm>> GetSubMenusAsync(string groupKey, ClaimsPrincipal user);
        Task<IReadOnlyList<MenuItemVm>> GetSubMenusAsync(char groupKey, ClaimsPrincipal user);
    }

    // ViewModel สำหรับเมนูย่อย
    public class MenuItemVm
    {
        public string Code { get; set; } = "";      // เช่น mclient, tnew, rinvoice
        public string Name { get; set; } = "";      // ชื่อแสดงผล
        public string Controller { get; set; } = ""; // ชื่อคอนโทรลเลอร์
        public string Action { get; set; } = "Index";
        public bool Enabled { get; set; } = false;   // true = 1 ใน security_menu
    }
}
