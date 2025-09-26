// ===== FILE: Services/IMenuGate.cs =====
using Ams.Media.Web.Models;

namespace Ams.Media.Web.Services
{
    /// <summary>
    /// สร้างเมนูย่อยตามกลุ่มตัวอักษร (M/T/R/E/A/S) จากสิทธิ์ใน Security_Menu ของผู้ใช้
    /// ตัวอย่าง: 'M' = Master files (mclient, mproduct, mvendor, ...)
    /// </summary>
    public interface IMenuGate
    {
        /// <summary>
        /// คืนรายการเมนูย่อยของกลุ่มที่ระบุ สำหรับผู้ใช้ที่กำหนด
        /// </summary>
        /// <param name="group">ตัวอักษรกลุ่มเมนู: 'M' | 'T' | 'R' | 'E' | 'A' | 'S'</param>
        /// <param name="username">ชื่อผู้ใช้ (ใช้แมตช์กับ Security_Menu.Username)</param>
        /// <returns>ลิสต์เมนูพร้อม Text/Url/Icon ตามสิทธิ์</returns>
        Task<IReadOnlyList<MenuItemVm>> GetSubMenusAsync(char group, string username);
    }
}
