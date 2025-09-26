using System.Data;
using Ams.Media.Web.Data;
using Ams.Media.Web.Dtos;
using Ams.Media.Web.Repositories.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Ams.Media.Web.Repositories;

public sealed class ClientRepository : IClientRepository
{
    private readonly DbConnectionFactory _db;
    public ClientRepository(DbConnectionFactory db) => _db = db;

    public async Task<IReadOnlyList<ClientUsageRow>> GetUsageAsync(int clientCode, CancellationToken ct)
    {
        const string sql = @"
SELECT 'TransactionMaster' AS Module, COUNT(*) AS [Count] FROM TransactionMaster WHERE ClientCode = @ClientCode
UNION ALL SELECT 'TransactionKey', COUNT(*) FROM TransactionKey WHERE ClientCode = @ClientCode
UNION ALL SELECT 'PlanDetail', COUNT(*) FROM PlanDetail WHERE ClientCode = @ClientCode
UNION ALL SELECT 'Product', COUNT(*) FROM Product WHERE ClientCode = @ClientCode
UNION ALL SELECT 'Campaign', COUNT(*) FROM Campaign WHERE ClientCode = @ClientCode
UNION ALL SELECT 'Material', COUNT(*) FROM Material WHERE ClientCode = @ClientCode
UNION ALL SELECT 'RateCode', COUNT(*) FROM RateCode WHERE ClientCode = @ClientCode
UNION ALL SELECT 'RateItem', COUNT(*) FROM RateItem WHERE ClientCode = @ClientCode
UNION ALL SELECT 'Booking', COUNT(*) FROM Booking WHERE ClientCode = @ClientCode
UNION ALL SELECT 'Job', COUNT(*) FROM Job WHERE ClientCode = @ClientCode;";
        using var con = _db.Create();
        var rows = await con.QueryAsync<ClientUsageRow>(new CommandDefinition(sql, new { ClientCode = clientCode }, cancellationToken: ct));
        return rows.AsList();
    }

    public async Task<int> InsertClientAsync(ClientDto dto, CancellationToken ct)
    {
        const string sql = @"
INSERT INTO Clients (ClientCode, ClieName, AgencyCom, ClientPrefix, ClientBranch, ClientTaxNo, CreditTerm, ClientStatus)
VALUES (@ClientCode, @ClieName, @AgencyCom, @ClientPrefix, @ClientBranch, @ClientTaxNo, @CreditTerm, @ClientStatus);";
        using var con = _db.Create();

        if (dto.ClientCode is null)
            throw new InvalidOperationException("ClientCode is required (ClientID = ClientCode).");

        await con.ExecuteAsync(new CommandDefinition(sql, new
        {
            ClientCode = dto.ClientCode,
            dto.ClieName,
            dto.AgencyCom,
            dto.ClientPrefix,
            dto.ClientBranch,
            dto.ClientTaxNo,
            dto.CreditTerm,
            dto.ClientStatus
        }, cancellationToken: ct));

        return dto.ClientCode.Value;
    }

    public async Task UpdateClientAsync(int clientCode, ClientDto dto, CancellationToken ct)
    {
        const string sql = @"
UPDATE Clients SET
  ClieName = @ClieName,
  AgencyCom = @AgencyCom,
  ClientPrefix = @ClientPrefix,
  ClientBranch = @ClientBranch,
  ClientTaxNo = @ClientTaxNo,
  CreditTerm = @CreditTerm,
  ClientStatus = @ClientStatus
WHERE ClientCode = @ClientCode;";
        using var con = _db.Create();
        await con.ExecuteAsync(new CommandDefinition(sql, new
        {
            ClientCode = clientCode,
            dto.ClieName,
            dto.AgencyCom,
            dto.ClientPrefix,
            dto.ClientBranch,
            dto.ClientTaxNo,
            dto.CreditTerm,
            dto.ClientStatus
        }, cancellationToken: ct));
    }

    public async Task UpsertAddressesAsync(int clientCode, IEnumerable<ClientAddressDto> addresses, CancellationToken ct)
    {
        using var con = _db.Create();
        await con.OpenAsync(ct);
        using var tx = con.BeginTransaction();

        try
        {
            await con.ExecuteAsync(new CommandDefinition(
                "DELETE FROM ClientAddress WHERE ClientCode = @ClientCode",
                new { ClientCode = clientCode }, tx, cancellationToken: ct));

            const string ins = @"
INSERT INTO ClientAddress (ClientCode, AddressType, Address1, StartDate, EndDate)
VALUES (@ClientCode, @AddressType, @Address1, @StartDate, @EndDate);";

            foreach (var a in addresses)
            {
                await con.ExecuteAsync(new CommandDefinition(ins, new
                {
                    ClientCode = clientCode,
                    a.AddressType,
                    a.Address1,
                    a.StartDate,
                    a.EndDate
                }, tx, cancellationToken: ct));
            }

            tx.Commit();
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    public async Task DeleteAllAddressesAsync(int clientCode, CancellationToken ct)
    {
        using var con = _db.Create();
        await con.ExecuteAsync(new CommandDefinition(
            "DELETE FROM ClientAddress WHERE ClientCode = @ClientCode",
            new { ClientCode = clientCode }, cancellationToken: ct));
    }

    public async Task<int> ExecClientDeleteSafeAsync(int clientCode, CancellationToken ct)
    {
        using var con = _db.Create();
        var p = new DynamicParameters();
        p.Add("@ClientCode", clientCode, DbType.Int32, ParameterDirection.Input);
        try
        {
            await con.ExecuteAsync(new CommandDefinition("Client_DeleteSafe", p, commandType: CommandType.StoredProcedure, cancellationToken: ct));
            return 1; // deleted
        }
        catch (SqlException ex) when (ex.Number >= 50000 || ex.Class >= 16)
        {
            return 0; // in use (RAISERROR)
        }
    }

    public async Task<bool> HasOverlapAsync(int clientCode, CancellationToken ct)
    {
        const string sql = @"
;WITH R AS (
  SELECT
    AddressType,
    CAST(StartDate AS date) AS S,
    CAST(ISNULL(EndDate, '9999-12-31') AS date) AS E
  FROM ClientAddress
  WHERE ClientCode = @ClientCode
)
SELECT TOP 1 1
FROM R a
JOIN R b
  ON a.AddressType = b.AddressType
 AND (a.S <= b.E AND b.S <= a.E)    -- inclusive overlap (ชนปลาย = ทับ)
 AND NOT (a.S = b.S AND a.E = b.E); -- กันกรณีเทียบแถวเดียวกัน
";
        using var con = _db.Create();
        var result = await con.ExecuteScalarAsync<int?>(new CommandDefinition(sql, new { ClientCode = clientCode }, cancellationToken: ct));
        return result.HasValue;
    }
}
