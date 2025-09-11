using System.ComponentModel.DataAnnotations.Schema;

namespace Ams.Media.Web.Models
{
    [Table("security_user")]
    public class SecurityUser
    {
        // �������� username �� key; ��ҵ��ҧ����� PK ��ԧ ����� HasNoKey() � DbContext ᷹
        public string? Username { get; set; }  // ���ͤ�������ԧ: username
        public string? Password { get; set; }  // password (�� plain/hashed ��� config)
        public string? Department { get; set; }

        // �Է�����ѡ (flag '1'/'0') � ***�� string ������***
        public string? Masterfiles { get; set; } // masterfiles
        public string? Transactions { get; set; } // transactions
        public string? Reports { get; set; } // reports
        public string? Enquirys { get; set; } // enquirys
        public string? Systems { get; set; } // systems
        public string? Addinss { get; set; } // addinss
        public string? Exports { get; set; } // exports
        public string? Approved { get; set; } // approved

        // �Է�������/��� � �ҡʤ�Ի��
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
