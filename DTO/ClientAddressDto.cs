namespace Ams.Media.Web.Dto
{
    public sealed class ClientAddressDto
    {
        public int ClientId { get; set; }
        public int AddressType { get; set; } // 1..5
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string? CompanyName { get; set; }
        public string? AddressName { get; set; }
        public string? AddressTitle { get; set; }

        public string? MultiAddress01 { get; set; }
        public string? MultiAddress02 { get; set; }
        public string? MultiAddress03 { get; set; }
        public string? MultiAddress04 { get; set; }

        public string? MultiAreaCode { get; set; }
        public string? ZipCode { get; set; }
        public string? MultiStateCode { get; set; }
        public string? MultiCountry { get; set; }
        public string? MultiComments { get; set; }
    }
}
