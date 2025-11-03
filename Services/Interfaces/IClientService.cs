using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ams.Media.Web.Dto;

namespace Ams.Media.Web.Services
{
    public interface IClientService
    {
        // ========= Client (ที่ ClientController ใช้) =========
        Task<long> GetNextClientIdAsync();
        Task<PagedResult<ClientGridRow>> SearchAsync(string from, string to, string showMode);
        Task<PagedResult<ClientGridRow>> SearchFreeAsync(string? q, string showMode);
        Task<ClientRow?> GetAsync(long id);
        Task<bool> CreateAsync(ClientRow input);
        Task<bool> UpdateAsync(ClientRow input);
        Task<bool> DeleteAsync(long id);

        // ========= Address =========
        Task<IReadOnlyList<ClientAddressDto>> AddressListAsync(int clientId, int? type, CancellationToken ct);
        Task<ClientAddressDto?> AddressGetAsync(int clientId, int type, DateTime start, DateTime? end, CancellationToken ct);
        Task<(bool ok, string? message, ClientAddressDto? echo)> AddressValidateAsync(
            ClientAddressDto input,
            (int clientId, int addressType, DateTime startDate)? oldKey,
            CancellationToken ct);
        Task<bool> AddressCreateAsync(ClientAddressDto input, CancellationToken ct);
        Task<bool> AddressUpdateAsync(int clientId, int addressType, DateTime oldStart, ClientAddressDto input, CancellationToken ct);
        Task<bool> AddressDeleteAsync(int clientId, int type, DateTime start, DateTime? end, CancellationToken ct);
    }
}
