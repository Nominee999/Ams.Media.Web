using System.Security.Claims;

namespace Ams.Media.Web.Services
{
    public interface IMenuGate
    {
        // �Է���������ѡ�ҡ Claims (menu:M/T/R/E/A/S)
        bool CanMasterfiles(ClaimsPrincipal user);
        bool CanTransactions(ClaimsPrincipal user);
        bool CanReports(ClaimsPrincipal user);
        bool CanEnquirys(ClaimsPrincipal user);
        bool CanAddins(ClaimsPrincipal user);
        bool CanSystems(ClaimsPrincipal user);

        // ��Ŵ�������µ�������: "M","T","R","E","S"
        Task<IReadOnlyList<MenuItemVm>> GetSubMenusAsync(string groupKey, ClaimsPrincipal user);
        Task<IReadOnlyList<MenuItemVm>> GetSubMenusAsync(char groupKey, ClaimsPrincipal user);
    }

    // ViewModel ����Ѻ��������
    public class MenuItemVm
    {
        public string Code { get; set; } = "";      // �� mclient, tnew, rinvoice
        public string Name { get; set; } = "";      // �����ʴ���
        public string Controller { get; set; } = ""; // ���ͤ͹��������
        public string Action { get; set; } = "Index";
        public bool Enabled { get; set; } = false;   // true = 1 � security_menu
    }
}
