namespace Ams.Media.Web.Models
{
    public class Client
    {
        public int ClientId { get; set; }            // PK
        public string? ClieName { get; set; }        // ชื่อลูกค้า
        public decimal? AgencyCom { get; set; }
        public string? ClientPrefix { get; set; }
        public string? ClientBranch { get; set; }
        public string? ClientTaxNo { get; set; }
        public string? BranchType { get; set; }
        public int? CreditTerm { get; set; }
        public string? ClientStatus { get; set; }    // ใช้แทน IsActive
    }
}
