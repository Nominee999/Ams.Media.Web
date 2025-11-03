// ===== FILE: DTO/ClientGridRow.cs =====
using System;

namespace Ams.Media.Web.Dto
{
    /// <summary>
    /// แถวข้อมูลสำหรับ Grid ของ Client (ให้ Dapper map ได้ด้วย ctor เปล่า + settable properties)
    /// </summary>
    public class ClientGridRow
    {
        public long ClientId { get; set; }
        public string ClientPrefix { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ClientTaxNo { get; set; } = string.Empty;
        public int ClientStatus { get; set; }

        // ฟิลด์เสริม (เผื่อ View ต้องใช้ – ไม่บังคับใน SELECT)
        public decimal? AgencyCom { get; set; }
        public int? CreditTerm { get; set; }
        public string? ClientBranch { get; set; }
        public int? BranchType { get; set; }
    }
}
