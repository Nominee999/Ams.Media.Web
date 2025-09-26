using Ams.Media.Web.Dtos;

namespace Ams.Media.Web.Repositories.Interfaces;

public interface IClientRepository
{
    Task<IReadOnlyList<ClientUsageRow>> GetUsageAsync(int clientCode, CancellationToken ct);

    Task<int> InsertClientAsync(ClientDto dto, CancellationToken ct);
    Task UpdateClientAsync(int clientCode, ClientDto dto, CancellationToken ct);
    Task UpsertAddressesAsync(int clientCode, IEnumerable<ClientAddressDto> addresses, CancellationToken ct);
    Task DeleteAllAddressesAsync(int clientCode, CancellationToken ct);

    Task<int> ExecClientDeleteSafeAsync(int clientCode, CancellationToken ct);
    Task<bool> HasOverlapAsync(int clientCode, CancellationToken ct);
}
