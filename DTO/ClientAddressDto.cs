// Dto/ClientAddressDto.cs
using System;

namespace Ams.Media.Web.Dto
{
    /// <summary>
    /// DTO สำหรับที่อยู่ลูกค้า ใช้แลกเปลี่ยนระหว่าง Controller/Service/View
    /// ไม่ผูก EF โดยตรง และไม่เพิ่มคอลัมน์ใหม่ใน DB
    /// </summary>
    public sealed class ClientAddressDto
    {
        public int ClientId { get; set; }
        public int AddressType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string? AddressTitle { get; set; }
        public string? Address01 { get; set; }
        public string? Address02 { get; set; }
        public string? Address03 { get; set; }
        public string? Address04 { get; set; }

        /// <summary>
        /// ให้ View เรียกใช้ชื่อฟิลด์ AddressName ได้ โดย map จาก AddressTitle (ไม่แตะ DB)
        /// </summary>
        public string? AddressName
        {
            get => AddressTitle;
            set => AddressTitle = value;
        }

        public DateTime GetEndOrMax() => EndDate ?? new DateTime(9999, 12, 31);
    }
}
