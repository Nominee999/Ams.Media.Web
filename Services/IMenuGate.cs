// ===== FILE: Services/IMenuGate.cs =====
using Ams.Media.Web.Models;

namespace Ams.Media.Web.Services
{
    /// <summary>
    /// ���ҧ�������µ�����������ѡ�� (M/T/R/E/A/S) �ҡ�Է���� Security_Menu �ͧ�����
    /// ������ҧ: 'M' = Master files (mclient, mproduct, mvendor, ...)
    /// </summary>
    public interface IMenuGate
    {
        /// <summary>
        /// �׹��¡���������¢ͧ���������к� ����Ѻ��������˹�
        /// </summary>
        /// <param name="group">����ѡ�á��������: 'M' | 'T' | 'R' | 'E' | 'A' | 'S'</param>
        /// <param name="username">���ͼ���� (�������Ѻ Security_Menu.Username)</param>
        /// <returns>��ʵ����پ���� Text/Url/Icon ����Է���</returns>
        Task<IReadOnlyList<MenuItemVm>> GetSubMenusAsync(char group, string username);
    }
}
