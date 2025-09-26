using Ams.Media.Web.Dtos;
using Ams.Media.Web.Repositories.Interfaces;
using Ams.Media.Web.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Ams.Media.Web.Data;

namespace Ams.Media.Web.Services;

// Primary constructor (C# 12)
public sealed class ClientService(IClientRepository repo, IConfiguration cfg) : IClientService
{
    public Task<IReadOnlyList<ClientUsageRow>> GetUsageAsync(int clientCode, CancellationToken ct = default)
        => repo.GetUsageAsync(clientCode, ct);

    public async Task<int> SaveAsync(ClientDto dto, CancellationToken ct = default)
    {
        // CA1512: Throw helper ᷹ if/throw
        ArgumentNullException.ThrowIfNull(dto.ClientCode);
        ValidateAddressRules(dto.Addresses);

        var code = await repo.InsertClientAsync(dto, ct);
        await repo.UpsertAddressesAsync(code, dto.Addresses, ct);

        if (await repo.HasOverlapAsync(code, ct))
        {
            await repo.DeleteAllAddressesAsync(code, ct);
            throw new InvalidOperationException("Address periods overlap (inclusive).");
        }
        return code;
    }

    public async Task<int> UpdateAsync(int clientCode, ClientDto dto, CancellationToken ct = default)
    {
        // CA1512: Throw helper
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(clientCode);
        ValidateAddressRules(dto.Addresses);

        await repo.UpdateClientAsync(clientCode, dto, ct);
        await repo.UpsertAddressesAsync(clientCode, dto.Addresses, ct);

        if (await repo.HasOverlapAsync(clientCode, ct))
            throw new InvalidOperationException("Address periods overlap (inclusive).");

        return clientCode;
    }

    public async Task<bool> DeleteSafeAsync(int clientCode, CancellationToken ct = default)
        => (await repo.ExecClientDeleteSafeAsync(clientCode, ct)) == 1;

    private static void ValidateAddressRules(IEnumerable<ClientAddressDto> addresses)
    {
        foreach (var a in addresses)
        {
            if (a.EndDate.HasValue && a.EndDate.Value.Date < a.StartDate.Date)
                throw new InvalidOperationException($"EndDate < StartDate for AddressType {a.AddressType}");
            if (a.AddressType is < 1 or > 5)
                throw new InvalidOperationException($"Invalid AddressType {a.AddressType} (must be 1..5)");
        }
    }

    // (optional) hybrid demo
    public async Task DemoEfPlusDapperAtomicAsync(int clientCode, CancellationToken ct = default)
    {
        var cs = cfg.GetConnectionString("AmsDb")!;
        await using var scope = await HybridDbScope.CreateAsync(cs, ct);

        var tx = scope.GetCurrentTransaction();
        _ = await repo.GetUsageAsync(clientCode, ct, tx);

        await scope.CommitAsync(ct);
    }
}
