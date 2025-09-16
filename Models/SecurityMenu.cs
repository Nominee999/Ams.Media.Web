using System.ComponentModel.DataAnnotations.Schema;

namespace Ams.Media.Web.Models
{
    [Table("security_menu")]
    public class SecurityMenu
    {
        // ฟิลด์เมนูย่อยทั้งหมดเป็น flag '1'/'0' (string)
        // ตัวอย่างหลัก ๆ (เติมเพิ่มได้ตามสคริปต์)
        public int Id { get; set; } // ถ้าตารางไม่มี identity ให้เอาออก และ HasNoKey() ใน DbContext

        // Masterfiles (ขึ้นต้น m…)
        public string? MCurrency { get; set; } // mcurrency
        public string? MClient { get; set; } // mclient
        public string? MCampaign { get; set; } // mcampaign
        public string? MProduct { get; set; } // mproduct
        public string? MStation { get; set; } // mstation
        public string? MTargetGroup { get; set; } // mtargetgroup
        public string? MLanguage { get; set; } // mlanguage
        public string? MMaterial { get; set; } // mmaterial
        public string? MMediaType { get; set; } // mmediatype
        public string? MVendor { get; set; } // mvendor
        public string? MRateCode { get; set; } // mratecode
        public string? MProgramType { get; set; } // mprogramtype
        public string? MRateItem { get; set; } // mrateitem
        public string? MBooking { get; set; } // mbooking
        public string? MTax { get; set; } // mtax
        public string? MDayPart { get; set; } // mdaypart

        // Transactions (ขึ้นต้น t…)
        public string? TNew { get; set; } // tnew
        public string? TDelete { get; set; } // tdelete
        public string? TBudget { get; set; } // tbudget (Define Schedule)
        public string? TSelectOrder { get; set; } // tselectorder
        public string? TSelectInvoice { get; set; } // tselectinvoice
        public string? TRefresh { get; set; } // trefresh

        // Reports (ขึ้นต้น r…)
        public string? RMaster { get; set; } // rmaster
        public string? RPurchase { get; set; } // rpurchase
        public string? RAmendment { get; set; } // ramendment
        public string? RInvoice { get; set; } // rinvoice
        public string? RScheduleFlow { get; set; } // rscheduleflow
        public string? RRevenue { get; set; } // rrevenue
        public string? RExpense { get; set; } // rexpense
        public string? RJob { get; set; } // rjob
        public string? RSchedule { get; set; } // rschedule
        public string? RPurchaseRpt { get; set; } // rpurchasereport
        public string? RBilling { get; set; } // rbilling
        public string? RAgencyMargin { get; set; } // rmargin
        public string? RMonitor { get; set; } // rmonitor

        // Enquiry (ขึ้นต้น e…)
        public string? EPurchase { get; set; } // epurchase
        public string? EInvoice { get; set; } // einvoice
        public string? ETVRating { get; set; } // etvrating
        public string? EReach { get; set; } // ereach

        // Systems (ขึ้นต้น s…)
        public string? SSetup { get; set; } // ssetup
        public string? SImportTVA { get; set; } // simporttvarating
        public string? SImportArena { get; set; } // simporttvarating_arena
        public string? SBackup { get; set; } // sbackup (ถ้ามี)
    }
}
