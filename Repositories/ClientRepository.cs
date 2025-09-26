using System.Data;
using Ams.Media.Web.Data;
using Ams.Media.Web.Dtos;
using Ams.Media.Web.Repositories.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Ams.Media.Web.Repositories;

// Primary constructor (C# 12)
public sealed class ClientRepository(DbConnectionFactory db) : IClientRepository
{
    public async Task<IReadOnlyList<ClientUsageRow>> GetUsageAsync(int clientCode, CancellationToken ct, IDbTransaction? tx = null)
    {
        // ตารางที่ต้องเช็ค (ชื่ออย่างเดียว ไม่ระบุสคีมา)
        var tables = new[]
        {
        "TransactionMaster","TransactionKey","PlanDetail","Product","Campaign",
        "Material","RateCode","RateItem","Booking","Job"
    };

        var results = new List<ClientUsageRow>(tables.Length);
        var con = tx?.Connection ?? db.Create();
        var disposeCon = tx is null;

        try
        {
            foreach (var tbl in tables)
            {
                try
                {
                    var sql = @"
DECLARE @schema sysname, @col sysname, @cnt int = 0;

-- หา schema ของตารางเป้าหมาย (ชื่อโต๊ะตรงตามที่ระบุ ไม่สนสคีมา)
SELECT TOP (1) @schema = s.name
FROM sys.tables t
JOIN sys.schemas s ON s.schema_id = t.schema_id
WHERE t.name = @tbl;

IF @schema IS NOT NULL
BEGIN
    -- เลือกคอลัมน์คีย์ลูกค้าที่มีอยู่จริง
    IF COL_LENGTH(QUOTENAME(@schema)+'.'+QUOTENAME(@tbl), 'ClientCode') IS NOT NULL
        SET @col = N'ClientCode';
    ELSE IF COL_LENGTH(QUOTENAME(@schema)+'.'+QUOTENAME(@tbl), 'ClientID') IS NOT NULL
        SET @col = N'ClientID';
    ELSE IF COL_LENGTH(QUOTENAME(@schema)+'.'+QUOTENAME(@tbl), 'Client_Id') IS NOT NULL
        SET @col = N'Client_Id';

    IF @col IS NOT NULL
    BEGIN
        DECLARE @sql nvarchar(max) =
            N'SELECT @cntOut = COUNT(*) FROM ' + QUOTENAME(@schema)+N'.'+QUOTENAME(@tbl) +
            N' WHERE ' + QUOTENAME(@col) + N' = @p';
        EXEC sp_executesql @sql, N'@p int, @cntOut int OUTPUT', @p = @clientCode, @cntOut = @cnt OUTPUT;
    END
END

SELECT @cnt;";

                    var cnt = await con.ExecuteScalarAsync<int>(new CommandDefinition(
                        sql, new { tbl, clientCode }, transaction: tx, cancellationToken: ct));

                    results.Add(new ClientUsageRow { Module = tbl, Count = cnt });
                }
                catch
                {
                    // ถ้าตาราง/สิทธิ์/ชื่อคอลัมน์มีปัญหา → คืน 0 แล้วไปตารางถัดไป
                    results.Add(new ClientUsageRow { Module = tbl, Count = 0 });
                }
            }

            return results;
        }
        finally
        {
            if (disposeCon) con.Dispose();
        }
    }




    public async Task<int> InsertClientAsync(ClientDto dto, CancellationToken ct)
    {
        const string sql = """
INSERT INTO Clients (ClientCode, ClieName, AgencyCom, ClientPrefix, ClientBranch, ClientTaxNo, CreditTerm, ClientStatus)
VALUES (@ClientCode, @ClieName, @AgencyCom, @ClientPrefix, @ClientBranch, @ClientTaxNo, @CreditTerm, @ClientStatus);
""";

        // CA1512: ใช้ Throw helper
        ArgumentNullException.ThrowIfNull(dto.ClientCode);

        using var con = db.Create();
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

        return dto.ClientCode!.Value;
    }

    public async Task UpdateClientAsync(int clientCode, ClientDto dto, CancellationToken ct)
    {
        const string sql = """
UPDATE Clients SET
  ClieName = @ClieName,
  AgencyCom = @AgencyCom,
  ClientPrefix = @ClientPrefix,
  ClientBranch = @ClientBranch,
  ClientTaxNo = @ClientTaxNo,
  CreditTerm = @CreditTerm,
  ClientStatus = @ClientStatus
WHERE ClientCode = @ClientCode;
""";

        using var con = db.Create();
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
        using var con = db.Create();
        con.Open(); // ใช้ Open() แทน OpenAsync() (แก้ error IDbConnection)

        using var tx = con.BeginTransaction();
        try
        {
            await con.ExecuteAsync(new CommandDefinition(
                "DELETE FROM ClientAddress WHERE ClientCode = @ClientCode",
                new { clientCode }, tx, cancellationToken: ct));

            const string ins = """
INSERT INTO ClientAddress (ClientCode, AddressType, Address1, StartDate, EndDate)
VALUES (@ClientCode, @AddressType, @Address1, @StartDate, @EndDate);
""";

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
        using var con = db.Create();
        await con.ExecuteAsync(new CommandDefinition(
            "DELETE FROM ClientAddress WHERE ClientCode = @ClientCode",
            new { clientCode }, cancellationToken: ct));
    }

    public async Task<int> ExecClientDeleteSafeAsync(int clientCode, CancellationToken ct)
    {
        using var con = db.Create();
        var p = new DynamicParameters();
        p.Add("@ClientCode", clientCode, DbType.Int32, ParameterDirection.Input);
        try
        {
            await con.ExecuteAsync(new CommandDefinition("Client_DeleteSafe", p, commandType: CommandType.StoredProcedure, cancellationToken: ct));
            return 1;
        }
        catch (SqlException ex) when (ex.Number >= 50000 || ex.Class >= 16)
        {
            return 0;
        }
    }

    public async Task<bool> HasOverlapAsync(int clientCode, CancellationToken ct)
    {
        const string sql = """
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
 AND (a.S <= b.E AND b.S <= a.E)
 AND NOT (a.S = b.S AND a.E = b.E);
""";
        using var con = db.Create();
        var result = await con.ExecuteScalarAsync<int?>(new CommandDefinition(sql, new { clientCode }, cancellationToken: ct));
        return result.HasValue;
    }
}
