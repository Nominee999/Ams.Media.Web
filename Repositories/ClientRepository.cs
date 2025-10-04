using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Ams.Media.Web.Dto;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Ams.Media.Web.Repositories
{
    public interface IClientRepository
    {
        Task<IEnumerable<ClientRow>> FindByNameAsync(string from, string to, string showMode);
        Task<IEnumerable<ClientRow>> FindByIdAsync(long id);
        Task<long> AddAsync(ClientRow input);
        Task<bool> UpdateAsync(ClientRow input);
        Task<(bool Success, string? Message)> DeleteFlagAsync(long id);
        Task<bool> CheckUsedInTransactionsAsync(long clientId);

        // ★ ใหม่: ค้นหาอิสระ (ClientID / ClieName / ClientPrefix / ClientTaxNo)
        Task<IEnumerable<ClientRow>> SearchFreeAsync(string? q, string showMode);
    }

    public sealed class ClientRepository : IClientRepository
    {
        private readonly string _conn;

        public ClientRepository(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("AmsDb")
                ?? throw new InvalidOperationException("ConnectionStrings:AmsDb not found.");
        }

        private IDbConnection Open() => new SqlConnection(_conn);

        // ใช้คลาส raw (nullable) เพื่อรับค่าจาก DB/SP อย่างปลอดภัย
        private sealed class RawClientRow
        {
            public long? CLIENTID { get; set; }
            public string? CLIENAME { get; set; }
            public decimal? AGENCYCOM { get; set; }
            public string? CLIENTPREFIX { get; set; }
            public int? CREDITTERM { get; set; }
            public string? CLIENTTAXNO { get; set; }
        }

        private static ClientRow MapToDto(RawClientRow r) => new ClientRow
        {
            ClientId = r.CLIENTID ?? 0L,
            Description = r.CLIENAME ?? string.Empty,
            AgencyCom = r.AGENCYCOM ?? 0m,
            ClientPrefix = r.CLIENTPREFIX ?? string.Empty,
            CreditTerm = r.CREDITTERM ?? 0
        };

        public async Task<IEnumerable<ClientRow>> FindByNameAsync(string from, string to, string showMode)
        {
            using var con = Open();

            var raws = await con.QueryAsync<RawClientRow>(
                "sp_find_NameRange",
                new { sType = "CLIENT", sFrom = from ?? string.Empty, sTo = to ?? string.Empty, sShow = showMode ?? "C" },
                commandType: CommandType.StoredProcedure);

            var list = raws.Select(MapToDto).ToList();

            foreach (var x in list)
            {
                x.IsNotUse = !string.IsNullOrWhiteSpace(x.Description)
                             && x.Description.IndexOf("NOT USE", StringComparison.OrdinalIgnoreCase) >= 0;

                if (x.ClientId > 0)
                {
                    var cnt = await con.ExecuteScalarAsync<int>(
                        "SELECT COUNT(*) FROM transactionkeymaster WHERE clientcode=@id",
                        new { id = x.ClientId });
                    x.IsInUse = cnt > 0;
                }
                else
                {
                    x.IsInUse = false;
                }
            }

            if ((showMode ?? "C").Equals("C", StringComparison.OrdinalIgnoreCase))
                list = list.Where(x => !x.IsNotUse).ToList();

            return list;
        }

        public async Task<IEnumerable<ClientRow>> FindByIdAsync(long id)
        {
            using var con = Open();

            var raws = await con.QueryAsync<RawClientRow>(
                "sp_find_ID",
                new { sType = "CLIENT", sID = id },
                commandType: CommandType.StoredProcedure);

            var list = raws.Select(MapToDto).ToList();

            foreach (var x in list)
            {
                x.IsNotUse = !string.IsNullOrWhiteSpace(x.Description)
                             && x.Description.IndexOf("NOT USE", StringComparison.OrdinalIgnoreCase) >= 0;

                if (x.ClientId > 0)
                {
                    var cnt = await con.ExecuteScalarAsync<int>(
                        "SELECT COUNT(*) FROM transactionkeymaster WHERE clientcode=@id",
                        new { id = x.ClientId });
                    x.IsInUse = cnt > 0;
                }
                else
                {
                    x.IsInUse = false;
                }
            }

            return list;
        }

        // ★ ใหม่: ค้นหาฟรีเท็กซ์ยิง DB ตรง ๆ ตาม 4 ฟิลด์
        public async Task<IEnumerable<ClientRow>> SearchFreeAsync(string? q, string showMode)
        {
            using var con = Open();

            // ถ้าไม่ใส่คำค้น ให้คืนลิสต์ช่วงชื่อแบบเดิม (กันหน้าโล่ง)
            if (string.IsNullOrWhiteSpace(q))
                return await FindByNameAsync("", "", showMode);

            var term = (q ?? "").Trim();

            const string sql = @"
SELECT
    ClientID   AS CLIENTID,
    ClieName   AS CLIENAME,
    AgencyCom  AS AGENCYCOM,
    ClientPrefix AS CLIENTPREFIX,
    CreditTerm AS CREDITTERM,
    ClientTaxNo AS CLIENTTAXNO
FROM dbo.Client WITH (NOLOCK)
WHERE
    (
        CONVERT(NVARCHAR(50), ClientID) LIKE '%' + @q + '%' OR
        ClieName      LIKE '%' + @q + '%' OR
        ClientPrefix  LIKE '%' + @q + '%' OR
        ClientTaxNo   LIKE '%' + @q + '%'
    )
    AND (@show <> 'C' OR ClieName NOT LIKE '%NOT USE%')
ORDER BY ClientID;";

            var raws = await con.QueryAsync<RawClientRow>(sql, new { q = term, show = (showMode ?? "C").ToUpperInvariant() });

            var list = raws.Select(MapToDto).ToList();

            // ธงแสดงผล
            foreach (var x in list)
            {
                x.IsNotUse = !string.IsNullOrWhiteSpace(x.Description)
                             && x.Description.IndexOf("NOT USE", StringComparison.OrdinalIgnoreCase) >= 0;

                if (x.ClientId > 0)
                {
                    var cnt = await con.ExecuteScalarAsync<int>(
                        "SELECT COUNT(*) FROM transactionkeymaster WHERE clientcode=@id",
                        new { id = x.ClientId });
                    x.IsInUse = cnt > 0;
                }
            }

            return list;
        }

        public async Task<long> AddAsync(ClientRow input)
        {
            if (input.AgencyCom < 0 || input.AgencyCom > 100)
                throw new ArgumentOutOfRangeException(nameof(input.AgencyCom), "AgencyCom must be between 0 and 100.");
            if (input.CreditTerm < 0 || input.CreditTerm > 365)
                throw new ArgumentOutOfRangeException(nameof(input.CreditTerm), "CreditTerm must be between 0 and 365.");

            using var con = Open();
            var p = new DynamicParameters();
            p.Add("@ret", dbType: DbType.String, size: 100, direction: ParameterDirection.InputOutput);
            p.Add("@DirectoryId", 0);
            p.Add("@DirName", (input.Description ?? string.Empty).Trim());
            p.Add("@AgencyCom", input.AgencyCom);
            p.Add("@ClientPrefix", (input.ClientPrefix ?? string.Empty).Trim());
            p.Add("@CreditTerm", input.CreditTerm);

            await con.ExecuteAsync("sp_add_client", p, commandType: CommandType.StoredProcedure);

            var ret = p.Get<string>("@ret");
            if (long.TryParse(ret, out var newId)) return newId;

            var q = @"SELECT TOP(1) ClientID FROM Client WITH (NOLOCK)
                      WHERE ClieName = @n ORDER BY ClientID DESC";
            return await con.ExecuteScalarAsync<long>(q, new { n = (input.Description ?? string.Empty).Trim() });
        }

        public async Task<bool> UpdateAsync(ClientRow input)
        {
            if (input.AgencyCom < 0 || input.AgencyCom > 100)
                throw new ArgumentOutOfRangeException(nameof(input.AgencyCom), "AgencyCom must be between 0 and 100.");
            if (input.CreditTerm < 0 || input.CreditTerm > 365)
                throw new ArgumentOutOfRangeException(nameof(input.CreditTerm), "CreditTerm must be between 0 and 365.");

            using var con = Open();
            var p = new DynamicParameters();
            p.Add("@ret", dbType: DbType.String, size: 100, direction: ParameterDirection.InputOutput);
            p.Add("@DirectoryId", input.ClientId);
            p.Add("@DirName", (input.Description ?? string.Empty).Trim());
            p.Add("@AgencyCom", input.AgencyCom);
            p.Add("@ClientPrefix", (input.ClientPrefix ?? string.Empty).Trim());
            p.Add("@CreditTerm", input.CreditTerm);

            await con.ExecuteAsync("sp_update_client", p, commandType: CommandType.StoredProcedure);
            var ret = p.Get<string>("@ret");
            return string.IsNullOrWhiteSpace(ret) || long.TryParse(ret, out _);
        }

        public async Task<(bool Success, string? Message)> DeleteFlagAsync(long id)
        {
            using var con = Open();
            var p = new DynamicParameters();
            p.Add("@ret", dbType: DbType.String, size: 200, direction: ParameterDirection.InputOutput);
            p.Add("@DirectoryId", id);
            await con.ExecuteAsync("sp_delete_client", p, commandType: CommandType.StoredProcedure);
            var ret = p.Get<string>("@ret");
            if (string.IsNullOrWhiteSpace(ret) || ret == "0")
                return (true, null);
            return (false, ret);
        }

        public async Task<bool> CheckUsedInTransactionsAsync(long clientId)
        {
            using var con = Open();
            if (clientId <= 0) return false;
            var cnt = await con.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM transactionkeymaster WHERE clientcode=@id",
                new { id = clientId });
            return cnt > 0;
        }
    }
}
