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
        // ตารางที่ต้องเช็ค
        var tables = new[]
        {
        "TransactionMaster","TransactionKey","PlanDetail","Product","Campaign",
        "Material","RateCode","RateItem","Booking","Job"
    };

        // ชื่อคอลัมน์ที่อาจใช้เป็นคีย์ลูกค้าในแต่ละตาราง (เรียงลำดับความน่าจะเป็น)
        var keyCandidates = new[] { "ClientCode", "ClientID", "Client_Id" };

        static string Q(string ident) => $"[{ident.Replace("]", "]]")}]";

        var con = tx?.Connection ?? db.Create();
        var disposeCon = tx is null; // ถ้าไม่มี tx จากภายนอก เราจะเป็นคนปิดเอง
        try
        {
            var result = new List<ClientUsageRow>(tables.Length);

            foreach (var tbl in tables)
            {
                // 1) หาว่าตารางนี้มีอยู่ไหม และมีคอลัมน์ไหนใช้กรองได้บ้าง
                var col = await con.ExecuteScalarAsync<string>(new CommandDefinition(
                    """
                SELECT TOP (1) COLUMN_NAME
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_NAME = @tbl
                  AND COLUMN_NAME IN (@c1, @c2, @c3)
                """,
                    new { tbl, c1 = keyCandidates[0], c2 = keyCandidates[1], c3 = keyCandidates[2] },
                    transaction: tx,
                    cancellationToken: ct
                ));

                int count = 0;
                if (!string.IsNullOrEmpty(col))
                {
                    // 2) นับจำนวนด้วยคอลัมน์ที่พบ (ป้องกัน SQL Injection เพราะชื่อมาจากลิสต์ในโค้ดและ escape เป็น identifier)
                    var sql = $"SELECT COUNT(*) FROM {Q(tbl)} WHERE {Q(col)} = @clientCode";
                    count = await con.ExecuteScalarAsync<int>(new CommandDefinition(
                        sql, new { clientCode }, transaction: tx, cancellationToken: ct));
                }
                // 3) เติมผลลัพธ์ (ถ้าไม่มีตาราง/ไม่มีคอลัมน์ที่ตรง → count = 0)
                result.Add(new ClientUsageRow { Module = tbl, Count = count });
            }

            return result;
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
