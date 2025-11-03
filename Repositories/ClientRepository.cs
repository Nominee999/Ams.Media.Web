// ===== FILE: Repositories/ClientRepository.cs =====
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ams.Media.Web.Data;                   // IDbConnectionFactory
using Ams.Media.Web.Dto;                    // ClientGridRow, ClientRow, PagedResult<T>
using Ams.Media.Web.Repositories.Interfaces;
using Dapper;

namespace Ams.Media.Web.Repositories
{
    public sealed class ClientRepository : IClientRepository
    {
        private readonly IDbConnectionFactory _factory;

        public ClientRepository(IDbConnectionFactory factory)
        {
            _factory = factory;
        }

        // รองรับ factory หลายชื่อเมธอดด้วย dynamic
        private IDbConnection CreateConn()
        {
            dynamic f = _factory;
            try { var c = (IDbConnection)f.CreateDbConnection(); if (c != null) return c; } catch { }
            try { var c = (IDbConnection)f.CreateConnection(); if (c != null) return c; } catch { }
            try { var c = (IDbConnection)f.Create(); if (c != null) return c; } catch { }
            throw new NotSupportedException("IDbConnectionFactory does not provide a known Create method (CreateDbConnection/CreateConnection/Create).");
        }

        private static void EnsureOpen(IDbConnection conn)
        {
            if (conn.State != ConnectionState.Open) conn.Open();
        }

        // ================= Implement IClientRepository =================

        public async Task<PagedResult<ClientGridRow>> ListPagedAsync(
            string? q, int page, int pageSize, string show, CancellationToken ct)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 50 : pageSize;

            // ใช้ temp table #src แทน CTE เพื่ออ้างได้หลายคำสั่ง
            // ชื่อคอลัมน์ตาม DB_SCript_V7.sql: ClieName, ClientStatus (char(1))
            const string sql = @"
IF OBJECT_ID('tempdb..#src') IS NOT NULL DROP TABLE #src;

SELECT
    c.ClientId,
    c.ClientPrefix,
    c.ClieName     AS Description,
    c.ClientTaxNo,
    CASE WHEN ISNULL(c.ClientStatus,'1') = '1' THEN 1 ELSE 0 END AS ClientStatus
INTO #src
FROM dbo.Client AS c WITH (NOLOCK)
WHERE
    (
        @Q IS NULL OR @Q = '' OR
        c.ClientId = TRY_CONVERT(BIGINT, @Q) OR
        c.ClieName     LIKE '%' + @Q + '%' OR
        c.ClientPrefix LIKE '%' + @Q + '%' OR
        c.ClientTaxNo  LIKE '%' + @Q + '%'
    )
    AND (
        @Show = 'A' OR ( @Show = 'C' AND ISNULL(c.ClientStatus,'1') = '1' )
    );

SELECT COUNT(*) FROM #src;

SELECT
    ClientId,
    ClientPrefix,
    Description,
    ClientTaxNo,
    ClientStatus
FROM #src
ORDER BY ClientId
OFFSET (@Page - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;
";

            using var conn = CreateConn();
            EnsureOpen(conn);

            var grid = await conn.QueryMultipleAsync(
                new CommandDefinition(
                    sql,
                    new { Q = q, Page = page, PageSize = pageSize, Show = show },
                    cancellationToken: ct));

            var total = await grid.ReadFirstAsync<int>();
            var items = (await grid.ReadAsync<ClientGridRow>()).ToList();

            return new PagedResult<ClientGridRow>(items, total, page, pageSize);
        }

        public async Task<long> GetNextClientIdAsync()
        {
            const string sql = @"SELECT ISNULL(MAX(c.ClientId), 0) + 1 FROM dbo.Client AS c;";

            using var conn = CreateConn();
            EnsureOpen(conn);

            return await conn.ExecuteScalarAsync<long>(sql);
        }

        public async Task<ClientRow?> GetAsync(long id)
        {
            // Map ClieName -> Description, แปลง ClientStatus(char) -> int
            const string sql = @"
SELECT
    c.ClientId,
    c.ClientPrefix,
    c.ClieName     AS Description,
    c.ClientTaxNo,
    CASE WHEN ISNULL(c.ClientStatus,'1') = '1' THEN 1 ELSE 0 END AS ClientStatus,
    c.AgencyCom,
    c.CreditTerm,
    c.ClientBranch,
    c.BranchType
FROM dbo.Client AS c
WHERE c.ClientId = @id;";

            using var conn = CreateConn();
            EnsureOpen(conn);

            return await conn.QuerySingleOrDefaultAsync<ClientRow>(sql, new { id });
        }

        public async Task<bool> CreateAsync(ClientRow input)
        {
            // เขียนลง ClieName จาก @Description และแปลงสถานะ int -> char(1)
            const string sql = @"
INSERT INTO dbo.Client
(
    ClientId,
    ClientPrefix,
    ClieName,
    ClientTaxNo,
    ClientStatus,
    AgencyCom,
    CreditTerm,
    ClientBranch,
    BranchType
)
VALUES
(
    @ClientId,
    @ClientPrefix,
    @Description,
    @ClientTaxNo,
    CASE WHEN @ClientStatus = 1 THEN '1' ELSE '0' END,
    @AgencyCom,
    @CreditTerm,
    @ClientBranch,
    @BranchType
);";

            using var conn = CreateConn();
            EnsureOpen(conn);

            var rows = await conn.ExecuteAsync(sql, input);
            return rows > 0;
        }

        public async Task<bool> UpdateAsync(ClientRow input)
        {
            // อัปเดต ClieName จาก @Description และแปลงสถานะ int -> char(1)
            const string sql = @"
UPDATE dbo.Client
SET
    ClientPrefix = @ClientPrefix,
    ClieName     = @Description,
    ClientTaxNo  = @ClientTaxNo,
    ClientStatus = CASE WHEN @ClientStatus = 1 THEN '1' ELSE '0' END,
    AgencyCom    = @AgencyCom,
    CreditTerm   = @CreditTerm,
    ClientBranch = @ClientBranch,
    BranchType   = @BranchType
WHERE ClientId   = @ClientId;";

            using var conn = CreateConn();
            EnsureOpen(conn);

            var rows = await conn.ExecuteAsync(sql, input);
            return rows > 0;
        }

        // Soft Delete ตามกติกา: เปลี่ยนเป็น Not Use ('0')
        public async Task<bool> DeleteAsync(long id)
        {
            const string sql = @"
UPDATE dbo.Client
SET ClientStatus = '0'
WHERE ClientId = @id;";

            using var conn = CreateConn();
            EnsureOpen(conn);

            var rows = await conn.ExecuteAsync(sql, new { id });
            return rows > 0;
        }
    }
}
