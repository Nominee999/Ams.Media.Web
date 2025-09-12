using System.ComponentModel.DataAnnotations.Schema;

namespace Ams.Media.Web.Models
{
    [Table("SECURITY_LOGFILE")]
    public class SecurityLogFile
    {
        public string? ProgramName { get; set; } // programname
        public string? Username { get; set; } // username
        public string? ComputerName { get; set; } // computername
        public string? Menu { get; set; } // menu
        public string? AuditType { get; set; } // audittype
        public DateTime? AuditDate { get; set; } // auditdate

        public string? M_ScheduleNo { get; set; } // m_scheduleno
        public string? M_OrderNo { get; set; } // m_orderno
        public string? M_InvoiceNo { get; set; } // m_invoiceno
        public string? P_JobNo { get; set; } // p_jobno
        public string? P_InvoiceNo { get; set; } // p_invoiceno

        public string? Fi_JournalNo { get; set; } // fi_journalno
        public string? Fi_PrevUser { get; set; } // fi_prevuser
        public DateTime? Fi_PrevDate { get; set; } // fi_prevdate

        public int? Item_Id { get; set; } // item_id
    }
}
