using System;

namespace Ams.Media.Web.Models
{
    // µÃ§¡ÑºÊ¤ÕÁèÒã¹ DB_SCript_V7.sql (Security_Menu)
    public class SecurityMenu
    {
        public string Username { get; set; } = string.Empty;

        // --- Masterfiles ---
        public string? Mcurrency { get; set; }
        public string? Mtarget { get; set; }
        public string? Mlanguage { get; set; }
        public string? Mstation { get; set; }
        public string? Mloading { get; set; }
        public string? Mmedia { get; set; }
        public string? Mclient { get; set; }
        public string? Mcampaign { get; set; }
        public string? Mproduct { get; set; }
        public string? Mvendor { get; set; }
        public string? Mratecode { get; set; }
        public string? Mprogramtype { get; set; }
        public string? Mrateitem { get; set; }
        public string? Mmaterial { get; set; }
        public string? Mtax { get; set; }
        public string? Mbooking { get; set; }
        public string? Mpackage { get; set; }
        public string? Mdaypart { get; set; }
        public string? Mcirculation { get; set; }
        public string? Mspottype { get; set; }
        public string? Mprogrammaster { get; set; }

        // --- Transactions ---
        public string? Tnew { get; set; }
        public string? Tdelete { get; set; }
        public string? Tbudget { get; set; }
        public string? Tselectinvoice { get; set; }
        public string? Tselectorder { get; set; }
        public string? Trefresh { get; set; }
        public string? Tdeleteitem { get; set; }
        public string? Tjob { get; set; }
        public string? Tschedule { get; set; }

        // --- Reports ---
        public string? Rmasterreport { get; set; }
        public string? Rpurchase { get; set; }
        public string? Ramendment { get; set; }
        public string? Rreports { get; set; }
        public string? Rinvoice { get; set; }
        public string? Rrevenue { get; set; }
        public string? Rexpense { get; set; }
        public string? Rbilling { get; set; }
        public string? Rmargin { get; set; }
        public string? Rchecklist { get; set; }
        public string? Rcheckreport { get; set; }
        public string? Rmonitor { get; set; }
        public string? Rorder { get; set; }
        public string? Rjob { get; set; }
        public string? Rschedule { get; set; }

        // --- Enquiry ---
        public string? Epurchase { get; set; }
        public string? Einvoice { get; set; }
        public string? Erating { get; set; }
        public string? Ereach { get; set; }

        // --- Systems / Imports / Etc. ---
        public string? Ssetup { get; set; }
        public string? Sbackup { get; set; }
        public string? Srating { get; set; }
        public string? Sreach { get; set; }
        public string? Saddins { get; set; }
        public string? Saddins1 { get; set; }
        public string? Saddins2 { get; set; }
        public string? Saddins3 { get; set; }
        public string? Saddins4 { get; set; }
        public string? Saddins5 { get; set; }
        public string? Saddins6 { get; set; }
        public string? Saddins7 { get; set; }
        public string? Saddins8 { get; set; }
        public string? Saddins9 { get; set; }
        public string? Saddins10 { get; set; }

        // --- Extra flags used in script ---
        public string? Exdtl { get; set; }
        public string? Extv3 { get; set; }
        public string? Extv7 { get; set; }
        public string? Pimport { get; set; }
        public string? Pmapping { get; set; }
        public string? Pmlist { get; set; }
        public string? Pmreport { get; set; }
        public string? Pearianna { get; set; }
        public string? Pematched { get; set; }
        public string? Pereport { get; set; }
        public string? Pereach { get; set; }
    }
}
