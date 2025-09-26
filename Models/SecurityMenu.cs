using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ams.Media.Web.Models
{
    [Table("Security_menu")]
    public class SecurityMenu
    {
        [Key]
        [Column("username")]
        [StringLength(40)]
        public string Username { get; set; } = "";

        // Masterfiles group
        [Column("mcurrency")] public string? MCurrency { get; set; }
        [Column("mtarget")] public string? MTarget { get; set; }
        [Column("mlanguage")] public string? MLanguage { get; set; }
        [Column("mstation")] public string? MStation { get; set; }
        [Column("mloading")] public string? MLoading { get; set; }
        [Column("mmedia")] public string? MMedia { get; set; }
        [Column("mclient")] public string? MClient { get; set; }
        [Column("mcampaign")] public string? MCampaign { get; set; }
        [Column("mproduct")] public string? MProduct { get; set; }
        [Column("mvendor")] public string? MVendor { get; set; }
        [Column("mratecode")] public string? MRateCode { get; set; }
        [Column("mprogramtype")] public string? MProgramType { get; set; }
        [Column("mrateitem")] public string? MRateItem { get; set; }
        [Column("mmaterial")] public string? MMaterial { get; set; }
        [Column("mtax")] public string? MTax { get; set; }
        [Column("mbooking")] public string? MBooking { get; set; }
        [Column("mpackage")] public string? MPackage { get; set; }
        [Column("mdaypart")] public string? MDayPart { get; set; }
        [Column("mcirculation")] public string? MCirculation { get; set; }

        // Transactions group
        [Column("tnew")] public string? TNew { get; set; }
        [Column("tdelete")] public string? TDelete { get; set; }
        [Column("tbudget")] public string? TBudget { get; set; }
        [Column("tselectinvoice")] public string? TSelectInvoice { get; set; }
        [Column("tselectorder")] public string? TSelectOrder { get; set; }
        [Column("trefresh")] public string? TRefresh { get; set; }

        // Reports group
        [Column("rmasterreport")] public string? RMasterReport { get; set; }
        [Column("rpurchase")] public string? RPurchase { get; set; }
        [Column("ramendment")] public string? RAmendment { get; set; }
        [Column("rreports")] public string? RReports { get; set; }
        [Column("rinvoice")] public string? RInvoice { get; set; }
        [Column("rrevenue")] public string? RRevenue { get; set; }
        [Column("rexpense")] public string? RExpense { get; set; }
        [Column("rbilling")] public string? RBilling { get; set; }
        [Column("rmargin")] public string? RMargin { get; set; }
        [Column("rchecklist")] public string? RChecklist { get; set; }
        [Column("rcheckreport")] public string? RCheckReport { get; set; }
        [Column("rmonitor")] public string? RMonitor { get; set; }

        // Enquiry / Extra
        [Column("epurchase")] public string? EPurchase { get; set; }
        [Column("einvoice")] public string? EInvoice { get; set; }
        [Column("erating")] public string? ERating { get; set; }

        // System / Addins
        [Column("ssetup")] public string? SSetup { get; set; }
        [Column("sbackup")] public string? SBackup { get; set; }
        [Column("srating")] public string? SRating { get; set; }
        [Column("saddins")] public string? SAddins { get; set; }
        [Column("saddins1")] public string? SAddins1 { get; set; }
        [Column("saddins2")] public string? SAddins2 { get; set; }
        [Column("saddins3")] public string? SAddins3 { get; set; }
        [Column("saddins4")] public string? SAddins4 { get; set; }
        [Column("saddins5")] public string? SAddins5 { get; set; }
        [Column("saddins6")] public string? SAddins6 { get; set; }
        [Column("saddins7")] public string? SAddins7 { get; set; }
        [Column("saddins8")] public string? SAddins8 { get; set; }
        [Column("saddins9")] public string? SAddins9 { get; set; }
        [Column("saddins10")] public string? SAddins10 { get; set; }

        // Extra Master flags
        [Column("mspottype")] public string? MSpotType { get; set; }
        [Column("mprogrammaster")] public string? MProgramMaster { get; set; }

        // Extra Transactions/Reports flags
        [Column("tdeleteitem")] public string? TDeleteItem { get; set; }
        [Column("rorder")] public string? ROrder { get; set; }
        [Column("ereach")] public string? EReach { get; set; }
        [Column("sreach")] public string? SReach { get; set; }
        [Column("tjob")] public string? TJob { get; set; }
        [Column("tschedule")] public string? TSchedule { get; set; }
        [Column("rjob")] public string? RJob { get; set; }
        [Column("rschedule")] public string? RSchedule { get; set; }

        // External / Import flags
        [Column("exdtl")] public string? ExDtl { get; set; }
        [Column("extv3")] public string? ExTv3 { get; set; }
        [Column("extv7")] public string? ExTv7 { get; set; }
        [Column("pimport")] public string? PImport { get; set; }
        [Column("pmapping")] public string? PMapping { get; set; }
        [Column("pmlist")] public string? PMList { get; set; }
        [Column("pmreport")] public string? PMReport { get; set; }
        [Column("pearianna")] public string? PEarianna { get; set; }
        [Column("pematched")] public string? PEMatched { get; set; }
        [Column("pereport")] public string? PEReport { get; set; }
        [Column("pereach")] public string? PEReach { get; set; }
    }
}
