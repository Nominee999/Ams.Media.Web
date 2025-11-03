// File: Dto/ClientRow.cs
using System.ComponentModel.DataAnnotations;
using Ams.Media.Web.Services.Validation;

namespace Ams.Media.Web.Dto
{
    public sealed class ClientRow
    {
        // 1) ReadOnly ที่ UI (ฝั่ง DB ใช้ค่า Max+1 ตอน New)
        public long ClientId { get; set; }

        // 3) ชื่อห้ามว่าง ยาว ≤ 50
        [Required, StringLength(50)]
        public string? Description { get; set; }

        // 5) ค่าตัวเลข ทศนิยมสูงสุด 3 (เริ่มต้น 0.000)
        [Range(0, 999999)]
        [RegularExpression(@"^\d+(\.\d{1,3})?$", ErrorMessage = "Agency Commission must have up to 3 decimals.")]
        public decimal AgencyCom { get; set; } = 0.000m;

        // 2) 2–4 ตัวอักษร/ตัวเลข
        [ClientPrefix]
        private string? _clientPrefix;
        public string? ClientPrefix
        {
            get => _clientPrefix;
            set => _clientPrefix = value?.Trim(); // << บังคับ Trim ที่ชั้น DTO
        }

        // 6) จำนวนเต็ม ≥ 0, default 30; ถ้ามากกว่า 30 ให้เตือนแต่อนุญาต (เตือนทำที่ Controller)
        [Range(0, int.MaxValue)]
        public int CreditTerm { get; set; } = 30;

        // 4) ค่าเริ่มต้น "สำนักงานใหญ่" แต่แก้ได้
        [StringLength(100)]
        public string ClientBranch { get; set; } = "สำนักงานใหญ่";

        // 7) ถ้าใส่ต้องเป็นตัวเลขติดกัน 13 หลัก; ว่างได้
        [ClientTaxNo]
        public string? ClientTaxNo { get; set; }

        // 8) Dropdown 0/1, default 0
        [Range(0, 1)]
        public int ClientStatus { get; set; } = 0;

        // 9) ตัวเลข default 0 (ซ่อนไม่โชว์)
        public int BranchType { get; set; } = 0;

        // สำหรับการแสดงผล/กรองฝั่ง UI ตาม VB6 เดิม
        public bool IsInUse { get; set; }
        public bool IsNotUse { get; set; }
    }
}
