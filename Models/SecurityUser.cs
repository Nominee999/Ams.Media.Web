using System.ComponentModel.DataAnnotations.Schema;

namespace Ams.Media.Web.Models
{
    [Table("security_user")]
    public class SecurityUser
    {
        [Column("username")] public string? Username { get; set; }
        [Column("password")] public string? Password { get; set; }
        [Column("department")] public string? Department { get; set; }

        // �Է�����ѡ
        [Column("masterfiles")] public string? Masterfiles { get; set; }
        [Column("transactions")] public string? Transactions { get; set; }
        [Column("reports")] public string? Reports { get; set; }
        [Column("enquirys")] public string? Enquirys { get; set; }
        [Column("systems")] public string? Systems { get; set; }
        [Column("addinss")] public string? Addinss { get; set; }
        [Column("exports")] public string? Exports { get; set; }
        [Column("approved")] public string? Approved { get; set; }

        // ��� � (�ִ���ʤ�Ի�������/��������辺)
        [Column("addnew")] public string? Addnew { get; set; }
        [Column("modify")] public string? Modify { get; set; }
        [Column("deleted")] public string? Deleted { get; set; }
        [Column("change")] public string? Change { get; set; }
        [Column("levels")] public string? Levels { get; set; }
        [Column("utility")] public string? Utility { get; set; }
        [Column("report")] public string? Report { get; set; }
        [Column("finance")] public string? Finance { get; set; }
        [Column("postcheck")] public string? Postcheck { get; set; }
        [Column("sections")] public string? Sections { get; set; }
    }
}
