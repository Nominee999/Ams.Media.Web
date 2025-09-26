//options/amsoptions.cs กำหนดค่าเริ่มต้นใหม่ ตามที่สั่ง 
namespace Ams.Media.Web.Options

{
    public sealed class AmsOptions
    {
        public bool EnableMaxUser { get; set; } = true;
        public int MaxUser { get; set; } = 8;

        // ค่าเริ่มต้นใหม่ตามที่กำหนด
        public int ReportClientId { get; set; } = 1;               // gReportClientID
        public string GroupCompanyName { get; set; } = "BBK";      // gGroupCompanyName
        public string ExpireDate { get; set; } = "31/12/2025";     // EXPIRE_DATE (เก็บเป็น string ตามที่สั่ง)
    }
}
