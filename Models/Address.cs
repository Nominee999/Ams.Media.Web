// Models/Address.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ams.Media.Web.Models
{
    [Table("Address")]                     // dbo.Address
    public sealed class Address
    {
        // === Composite Key ===
        // PK ที่เหมาะสม: (ClientCode, AddressType, StartDate, EndDate)
        // จะกำหนดจริงใน OnModelCreating อีกที
        [Column("ClientCode")]
        public int ClientCode { get; set; }         // FK -> Client.ClientID

        [Column("AddressType")]
        public int AddressType { get; set; }        // 1..5

        [Column("StartDate")]
        public DateTime StartDate { get; set; }

        [Column("EndDate")]
        public DateTime EndDate { get; set; }

        // ====== คอลัมน์เนื้อหาตามสคริปต์ V7 (ยึดชื่อใน repo ปัจจุบัน) ======
        [Column("CompanyName"), StringLength(255)]
        public string? CompanyName { get; set; }

        [Column("AddressName"), StringLength(255)]
        public string? AddressName { get; set; }

        [Column("AddressTitle"), StringLength(255)]
        public string? AddressTitle { get; set; }

        [Column("MultiAddress01"), StringLength(255)]
        public string? MultiAddress01 { get; set; }

        [Column("MultiAddress02"), StringLength(255)]
        public string? MultiAddress02 { get; set; }

        [Column("MultiAddress03"), StringLength(255)]
        public string? MultiAddress03 { get; set; }

        [Column("MultiAddress04"), StringLength(255)]
        public string? MultiAddress04 { get; set; }

        [Column("MultiAreaCode"), StringLength(50)]
        public string? MultiAreaCode { get; set; }

        [Column("ZipCode"), StringLength(20)]
        public string? ZipCode { get; set; }

        [Column("MultiStateCode"), StringLength(50)]
        public string? MultiStateCode { get; set; }

        [Column("MultiCountry"), StringLength(100)]
        public string? MultiCountry { get; set; }

        [Column("MultiComments"), StringLength(500)]
        public string? MultiComments { get; set; }
    }
}
