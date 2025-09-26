using Ams.Media.Web.Dtos;
using Ams.Media.Web.Repositories.Interfaces;
using Ams.Media.Web.Services.Interfaces;

namespace Ams.Media.Web.Services;

public sealed class ClientService : IClientService
{
    private readonly IClientRepository _repo;

    public ClientService(IClientRepository repo) => _repo = repo;

    public Task<IReadOnlyList<ClientUsageRow>> GetUsageAsync(int clientCode, CancellationToken ct = default)
        => _repo.GetUsageAsync(clientCode, ct);

    public async Task<int> SaveAsync(ClientDto dto, CancellationToken ct = default)
    {
        if (dto.ClientCode is null)
            throw new InvalidOperationException("ClientCode is required (ClientID = ClientCode).");

        ValidateAddressRules(dto.Addresses);

        var code = await _repo.InsertClientAsync(dto, ct);
        await _repo.UpsertAddressesAsync(code, dto.Addresses, ct);

        if (await _repo.HasOverlapAsync(code, ct))
        {
            await _repo.DeleteAllAddressesAsync(code, ct);
            throw new InvalidOperationException("Address periods overlap (inclusive).");
        }

        return code;
    }

    public async Task<int> UpdateAsync(int clientCode, ClientDto dto, CancellationToken ct = default)
    {
        if (clientCode <= 0) throw new ArgumentOutOfRangeException(nameof(clientCode));
        ValidateAddressRules(dto.Addresses);

        await _repo.UpdateClientAsync(clientCode, dto, ct);
        await _repo.UpsertAddressesAsync(clientCode, dto.Addresses, ct);

        if (await _repo.HasOverlapAsync(clientCode, ct))
            throw new InvalidOperationException("Address periods overlap (inclusive).");

        return clientCode;
    }

    public async Task<bool> DeleteSafeAsync(int clientCode, CancellationToken ct = default)
    {
        var ok = await _repo.ExecClientDeleteSafeAsync(clientCode, ct);
        return ok == 1;
    }

    private static void ValidateAddressRules(IEnumerable<ClientAddressDto> addresses)
    {
        foreach (var a in addresses)
        {
            if (a.EndDate.HasValue && a.EndDate.Value.Date < a.StartDate.Date)
                throw new InvalidOperationException($"EndDate < StartDate for AddressType {a.AddressType}");
            if (a.AddressType < 1 || a.AddressType > 5)
                throw new InvalidOperationException($"Invalid AddressType {a.AddressType} (must be 1..5)");
        }
    }
}
