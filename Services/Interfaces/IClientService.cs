using Ams.Media.Web.Dto;

namespace Ams.Media.Web.Services
{
    public interface IClientService
    {
        // ===== Client (สำหรับจอรายชื่อเดิม) =====
        Task<IEnumerable<ClientRow>> SearchAsync(string from, string to, string showMode);
        Task<ClientRow?> GetAsync(long id);
        Task<long> CreateAsync(ClientRow input);
        Task<bool> UpdateAsync(ClientRow input);
        Task<(bool Success, string? Message)> DeleteAsync(long id);

        /// <summary>
        /// Free-text search ยิง DB ตรง: ค้นได้จาก ClientID, Name/Description, ClientPrefix, ClientTaxId
        /// showMode: "C" = ซ่อน NOT USE, "A" = แสดงทั้งหมด
        /// </summary>
        Task<IEnumerable<ClientRow>> SearchFreeAsync(string? q, string showMode);

        // ===== Address (ใช้โดย ClientAddressesController) =====
        Task<IReadOnlyList<ClientAddressDto>> AddressListAsync(int clientId, int? type, CancellationToken ct);

        /// <summary>
        /// อ่านข้อมูลที่ระบุ clientId/addressType/start/(end) ถ้าไม่พบ คืน null
        /// </summary>
        Task<ClientAddressDto?> AddressGetAsync(int clientId, int addressType, DateTime start, DateTime? end, CancellationToken ct);

        /// <summary>
        /// ตรวจช่วงวันซ้อนกับรายการอื่น ๆ
        /// oldKey: ถ้าเป็นการแก้ไข ให้ระบุคีย์เดิม (clientId, addressType, startDate) เพื่อยกเว้นเรคอร์ดเดิมจากการชนกัน
        /// </summary>
        Task<(bool ok, string? message, IReadOnlyList<ClientAddressDto> conflicts)>
            AddressValidateAsync(ClientAddressDto dto,
                                 (int clientId, int addressType, DateTime startDate)? oldKey,
                                 CancellationToken ct);

        /// <summary>สร้างเรคอร์ดใหม่ (จะ throw/return false ถ้ามีชนช่วงวัน)</summary>
        Task<bool> AddressCreateAsync(ClientAddressDto dto, CancellationToken ct);

        /// <summary>
        /// แก้ไขเรคอร์ด: ระบุคีย์เดิมด้วย clientId/addressType/start เพื่อให้ระบบลบแถวเดิมก่อน upsert
        /// </summary>
        Task<bool> AddressUpdateAsync(int clientId, int addressType, DateTime start, ClientAddressDto dto, CancellationToken ct);

        /// <summary>ลบเรคอร์ดตามคีย์</summary>
        Task<bool> AddressDeleteAsync(int clientId, int addressType, DateTime start, DateTime? end, CancellationToken ct);
    }
}
