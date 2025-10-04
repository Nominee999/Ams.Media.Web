// File: Dto/ClientRow.cs
using System;

namespace Ams.Media.Web.Dto
{
    public sealed class ClientRow
    {
        public long ClientId { get; set; }
        public string? Description { get; set; }
        public decimal AgencyCom { get; set; }
        public string? ClientPrefix { get; set; }
        public int CreditTerm { get; set; }

        // สำหรับการแสดงผล/กรองฝั่ง UI ตามพฤติกรรม VB6 เดิม
        public bool IsInUse { get; set; }
        public bool IsNotUse { get; set; }
    }
}
