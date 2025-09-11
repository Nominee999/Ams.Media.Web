using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ams.Media.Web.Models
{
    [Table("Security_Log")]
    public class SecurityLog
    {
        [Key]
        [Column("username")]
        [StringLength(40)]
        public string Username { get; set; } = string.Empty;

        [Column("userdatetime")]
        public DateTime? UserDateTime { get; set; }

        [Column("computername")]
        [StringLength(50)]
        public string? ComputerName { get; set; }

        [Column("processing")]
        [StringLength(50)]
        public string? Processing { get; set; }
    }
}
