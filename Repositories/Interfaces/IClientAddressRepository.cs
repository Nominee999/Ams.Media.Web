using Ams.Media.Web.Dto;

namespace Ams.Media.Web.Repositories.Interfaces
{
    public interface IClientAddressRepository
    {
        Task<IReadOnlyList<ClientAddressDto>> ListAsync(int clientId, int? type, CancellationToken ct);
        Task<bool> ExistsOverlapAsync(int clientId, int addressType, DateTime start, DateTime end,
                                      DateTime? excludeStart, DateTime? excludeEnd, CancellationToken ct);
        Task UpsertAsync(ClientAddressDto dto, DateTime? oldStart, DateTime? oldEnd, CancellationToken ct);
        Task<bool> DeleteAsync(int clientId, int addressType, DateTime start, DateTime end, CancellationToken ct);
    }
}
