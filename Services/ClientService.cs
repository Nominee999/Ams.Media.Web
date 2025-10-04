using Ams.Media.Web.Dto;
using Ams.Media.Web.Repositories;
using Ams.Media.Web.Repositories.Interfaces;
using System.Linq;

namespace Ams.Media.Web.Services
{
    public sealed class ClientService : IClientService
    {
        private readonly IClientRepository _repo;
        private readonly IClientAddressRepository _addr;

        public ClientService(IClientRepository repo, IClientAddressRepository addr)
        {
            _repo = repo;
            _addr = addr;
        }

        // ===== Client =====

        public async Task<IEnumerable<ClientRow>> SearchAsync(string from, string to, string showMode)
        {
            var show = (showMode ?? "C").ToUpperInvariant();
            if (show != "A" && show != "C") show = "C";
            return await _repo.FindByNameAsync(from ?? string.Empty, to ?? string.Empty, show);
        }

        public async Task<ClientRow?> GetAsync(long id)
            => (await _repo.FindByIdAsync(id)).FirstOrDefault();

        public Task<long> CreateAsync(ClientRow input)
            => _repo.AddAsync(input);

        public Task<bool> UpdateAsync(ClientRow input)
            => _repo.UpdateAsync(input);

        public async Task<(bool Success, string? Message)> DeleteAsync(long id)
        {
            if (await _repo.CheckUsedInTransactionsAsync(id))
                return (false, "Client is in use by transactions.");
            return await _repo.DeleteFlagAsync(id);
        }

        public Task<IEnumerable<ClientRow>> SearchFreeAsync(string? q, string showMode)
        {
            var show = (showMode ?? "C").ToUpperInvariant();
            if (show != "A" && show != "C") show = "C";
            return _repo.SearchFreeAsync(q, show);
        }

        // ===== Address =====

        public Task<IReadOnlyList<ClientAddressDto>> AddressListAsync(int clientId, int? type, CancellationToken ct)
            => _addr.ListAsync(clientId, type, ct);

        public async Task<ClientAddressDto?> AddressGetAsync(int clientId, int addressType, DateTime start, DateTime? end, CancellationToken ct)
        {
            var list = await _addr.ListAsync(clientId, addressType, ct);
            // ถ้าผู้เรียกส่ง end มา ให้แมตช์แบบ start==start && end==end
            // ถ้าไม่ส่ง end ให้แมตช์แถวที่มี start ตรง และปล่อย end เป็นอะไรก็ได้ (จะเลือกตัวแรก)
            var q = list.Where(x => x.ClientId == clientId && x.AddressType == addressType && x.StartDate == start);
            if (end.HasValue) q = q.Where(x => x.EndDate == end.Value);
            return q.OrderBy(x => x.EndDate).FirstOrDefault();
        }

        public async Task<(bool ok, string? message, IReadOnlyList<ClientAddressDto> conflicts)>
            AddressValidateAsync(ClientAddressDto dto,
                                 (int clientId, int addressType, DateTime startDate)? oldKey,
                                 CancellationToken ct)
        {
            DateTime? excludeStart = null, excludeEnd = null;

            if (oldKey is not null)
            {
                // หาเรคอร์ดเดิมเพื่อทราบ EndDate เก่า (ใช้ยกเว้นตอนตรวจชน)
                var old = await AddressGetAsync(oldKey.Value.clientId, oldKey.Value.addressType, oldKey.Value.startDate, null, ct);
                if (old is not null)
                {
                    excludeStart = old.StartDate;
                    excludeEnd = old.EndDate;
                }
            }

            var overlap = await _addr.ExistsOverlapAsync(
                dto.ClientId, dto.AddressType,
                dto.StartDate, dto.EndDate,
                excludeStart, excludeEnd, ct);

            if (!overlap)
                return (true, null, Array.Empty<ClientAddressDto>());

            // สร้างรายการ conflicts แบบง่าย: เอาทุกรายการของ type เดียวกันที่คาบเกี่ยว
            var all = await _addr.ListAsync(dto.ClientId, dto.AddressType, ct);
            var conflicts = all
                .Where(x => !(x.EndDate < dto.StartDate || x.StartDate > dto.EndDate) &&
                            !(excludeStart.HasValue && excludeEnd.HasValue &&
                              x.StartDate == excludeStart.Value && x.EndDate == excludeEnd.Value))
                .OrderBy(x => x.StartDate)
                .ToList();

            return (false, "Date range overlaps with existing address periods.", conflicts);
        }

        public async Task<bool> AddressCreateAsync(ClientAddressDto dto, CancellationToken ct)
        {
            var (ok, _, _) = await AddressValidateAsync(dto, oldKey: null, ct);
            if (!ok) return false;

            await _addr.UpsertAsync(dto, oldStart: null, oldEnd: null, ct);
            return true;
        }

        public async Task<bool> AddressUpdateAsync(int clientId, int addressType, DateTime start, ClientAddressDto dto, CancellationToken ct)
        {
            // ยืนยันว่าแก้ของคนเดียวกัน/ประเภทเดียวกัน
            dto.ClientId = clientId;
            dto.AddressType = addressType;

            var old = await AddressGetAsync(clientId, addressType, start, null, ct);
            if (old is null) return false;

            var (ok, _, _) = await AddressValidateAsync(dto, (clientId, addressType, start), ct);
            if (!ok) return false;

            await _addr.UpsertAsync(dto, oldStart: old.StartDate, oldEnd: old.EndDate, ct);
            return true;
        }

        public Task<bool> AddressDeleteAsync(int clientId, int addressType, DateTime start, DateTime? end, CancellationToken ct)
        {
            // ถ้า caller ไม่ทราบ end ให้ลองหาเพื่อส่งค่าที่ถูกต้องให้ repo
            if (!end.HasValue)
            {
                return DeleteWithLookupAsync(clientId, addressType, start, ct);
            }
            return _addr.DeleteAsync(clientId, addressType, start, end.Value, ct);
        }

        private async Task<bool> DeleteWithLookupAsync(int clientId, int addressType, DateTime start, CancellationToken ct)
        {
            var row = await AddressGetAsync(clientId, addressType, start, null, ct);
            if (row is null) return false;
            return await _addr.DeleteAsync(clientId, addressType, row.StartDate, row.EndDate, ct);
        }
    }
}
