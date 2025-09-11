using System.ComponentModel.DataAnnotations;

namespace Ams.Media.Web.Models
{
    public class SecurityMenu
    {
        [Key]
        public string Username { get; set; } = "";   // PK (char(40))

        // Masterfiles (ขึ้นต้น m)
        public string? MCurrency { get; set; }
        public string? MTarget { get; set; }
        public string? MLanguage { get; set; }
        public string? MStation { get; set; }
        public string? MLoading { get; set; }
        public string? MMedia { get; set; }
        public string? MClient { get; set; }
        public string? MCampaign { get; set; }
        public string? MProduct { get; set; }
        public string? MVendor { get; set; }
        public string? MRateCode { get; set; }
        public string? MProgramType { get; set; }
        public string? MRateItem { get; set; }
        public string? MMaterial { get; set; }
        public string? MTax { get; set; }
        public string? MBooking { get; set; }
        public string? MPackage { get; set; }
        public string? MDayPart { get; set; }
        public string? MCirculation { get; set; }
        public string? MSpotType { get; set; }
        public string? MProgramMaster { get; set; }

        // Transactions (ขึ้นต้น t)
        public string? TNew { get; set; }
        public string? TDelete { get; set; }
        public string? TBudget { get; set; }
        public string? TSelectInvoice { get; set; }
        public string? TSelectOrder { get; set; }
        public string? TRefresh { get; set; }
        public string? TDeleteItem { get; set; }
        public string? TJob { get; set; }
        public string? TSchedule { get; set; }

        // Reports (ขึ้นต้น r)
        public string? RMasterReport { get; set; }
        public string? RPurchase { get; set; }
        public string? RAmendment { get; set; }
        public string? RReports { get; set; }
        public string? RInvoice { get; set; }
        public string? RRevenue { get; set; }
        public string? RExpense { get; set; }
        public string? RBilling { get; set; }
        public string? RMargin { get; set; }
        public string? RChecklist { get; set; }
        public string? RCheckReport { get; set; }
        public string? RMonitor { get; set; }
        public string? ROrder { get; set; }
        public string? RJob { get; set; }
        public string? RSchedule { get; set; }

        // Enquiry (ขึ้นต้น e)
        public string? EPurchase { get; set; }
        public string? EInvoice { get; set; }
        public string? ERating { get; set; }
        public string? EReach { get; set; }

        // Systems (ขึ้นต้น s)
        public string? SSetup { get; set; }
        public string? SBackup { get; set; }
        public string? SRating { get; set; }
        public string? SReach { get; set; }

        // Addins (saddins*)
        public string? SAddins { get; set; }
        public string? SAddins1 { get; set; }
        public string? SAddins2 { get; set; }
        public string? SAddins3 { get; set; }
        public string? SAddins4 { get; set; }
        public string? SAddins5 { get; set; }
        public string? SAddins6 { get; set; }
        public string? SAddins7 { get; set; }
        public string? SAddins8 { get; set; }
        public string? SAddins9 { get; set; }
        public string? SAddins10 { get; set; }

        // ส่วนเสริม (ex*, p*)
        public string? ExDtl { get; set; }
        public string? ExTv3 { get; set; }
        public string? ExTv7 { get; set; }

        public string? PImport { get; set; }
        public string? PMapping { get; set; }
        public string? PMList { get; set; }
        public string? PMReport { get; set; }
        public string? PEarianna { get; set; }
        public string? PEMatched { get; set; }
        public string? PEReport { get; set; }
        public string? PEReach { get; set; }
    }
}
