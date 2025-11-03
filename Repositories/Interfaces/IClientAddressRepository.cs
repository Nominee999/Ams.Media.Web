// Repositories/Interfaces/IClientAddressRepository.cs
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ams.Media.Web.Dto;

namespace Ams.Media.Web.Repositories.Interfaces
{
    public interface IClientAddressRepository
    {
        Task<IReadOnlyList<ClientAddressDto>> ListAsync(int clientId, int? addressType, CancellationToken ct);
        Task<ClientAddressDto?> GetAsync(int clientId, int addressType, DateTime start, DateTime? end, CancellationToken ct);

        Task<bool> IsOverlapAsync(
            int clientId,
            int addressType,
            DateTime start,
            DateTime? end,
            (int clientId, int addressType, DateTime startDate)? oldKey,
            CancellationToken ct);

        Task<bool> CreateAsync(ClientAddressDto dto, CancellationToken ct);
        Task<bool> UpdateAsync(int clientId, int addressType, DateTime oldStart, ClientAddressDto dto, CancellationToken ct);
        Task<bool> DeleteAsync(int clientId, int addressType, DateTime start, DateTime? end, CancellationToken ct);

        Task<bool> UpsertAsync(ClientAddressDto dto, DateTime? oldStart, DateTime? oldEnd, CancellationToken ct);
    }
}
