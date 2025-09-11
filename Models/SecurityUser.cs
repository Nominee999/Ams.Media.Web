using System.ComponentModel.DataAnnotations.Schema;

namespace Ams.Media.Web.Models
{
    [Table("security_user")]
    public class SecurityUser
    {
        // สมมติให้ username เป็น key; ถ้าตารางไม่มี PK จริง ให้ตั้ง HasNoKey() ใน DbContext แทน
        public string? Username { get; set; }  // ชื่อคอลัมน์จริง: username
        public string? Password { get; set; }  // password (เก็บ plain/hashed ตาม config)
        public string? Department { get; set; }

        // สิทธิ์หลัก (flag '1'/'0') — ***เป็น string ทั้งหมด***
        public string? Masterfiles { get; set; } // masterfiles
        public string? Transactions { get; set; } // transactions
        public string? Reports { get; set; } // reports
        public string? Enquirys { get; set; } // enquirys
        public string? Systems { get; set; } // systems
        public string? Addinss { get; set; } // addinss
        public string? Exports { get; set; } // exports
        public string? Approved { get; set; } // approved

        // สิทธิ์ย่อย/อื่น ๆ จากสคริปต์
        public string? Addnew { get; set; }
        public string? Modify { get; set; }
        public string? Deleted { get; set; }
        public string? Change { get; set; }
        public string? Levels { get; set; }
        public string? Utility { get; set; }
        public string? Report { get; set; }
        public string? Finance { get; set; }
        public string? Postcheck { get; set; }
        public string? Sections { get; set; }
    }
}
