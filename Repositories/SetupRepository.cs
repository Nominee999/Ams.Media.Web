using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Ams.Media.Web.Repositories.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Ams.Media.Web.Repositories
{
    public sealed class SetupRepository : ISetupRepository
    {
        private readonly string _connStr;
        public SetupRepository(IConfiguration config)
        {
            _connStr = config.GetConnectionString("AmsDb")
                      ?? config.GetConnectionString("Default")
                      ?? "";
        }

        public async Task<IReadOnlyList<(string Value, string Text, int Sort)>> GetOptionsAsync(string setupTypeKey, CancellationToken ct)
        {
            // 1) ลองรูปแบบคอลัมน์ชุดที่ 1 (SetupCode/SetupName/DisplaySeq/IsActive)
            const string sqlV1 = @"
SELECT
    CAST(st.SetupCode AS varchar(20))                    AS Value,
    st.SetupName                                         AS [Text],
    ISNULL(st.DisplaySeq, TRY_CAST(st.SetupCode AS int)) AS Sort
FROM SetupType st
WHERE st.SetupType = @setupTypeKey AND ISNULL(st.IsActive,1)=1
  AND TRY_CAST(st.SetupCode AS int) BETWEEN 1 AND 5
ORDER BY Sort;";

            // 2) ลองรูปแบบคอลัมน์ชุดที่ 2 (Code/Name/Sort/Active)
            const string sqlV2 = @"
SELECT
    CAST(st.[Code] AS varchar(20))                       AS Value,
    st.[Name]                                           AS [Text],
    ISNULL(st.[Sort], TRY_CAST(st.[Code] AS int))        AS Sort
FROM SetupType st
WHERE st.[Type] = @setupTypeKey AND ISNULL(st.[Active],1)=1
  AND TRY_CAST(st.[Code] AS int) BETWEEN 1 AND 5
ORDER BY Sort;";

            await using var conn = new SqlConnection(_connStr);
            if (conn.State != ConnectionState.Open) await conn.OpenAsync(ct);

            try
            {
                var rows1 = await conn.QueryAsync<(string Value, string Text, int Sort)>(
                    new CommandDefinition(sqlV1, new { setupTypeKey }, cancellationToken: ct, commandType: CommandType.Text));
                var list1 = rows1.AsList();
                if (list1.Count > 0) return list1;
            }
            catch (SqlException)
            {
                // ignore and try sqlV2
            }

            try
            {
                var rows2 = await conn.QueryAsync<(string Value, string Text, int Sort)>(
                    new CommandDefinition(sqlV2, new { setupTypeKey }, cancellationToken: ct, commandType: CommandType.Text));
                var list2 = rows2.AsList();
                if (list2.Count > 0) return list2;
            }
            catch (SqlException)
            {
                // ignore and fallback
            }

            // 3) Fallback: คืนค่าคงที่ 1..5 ตามสเปกโครงการ
            // NOTE: ถ้าคุณยืนยัน schema จริงได้เมื่อไร ค่อยกลับมาใช้ SQL เดียวให้ตรงฐาน
            var fallback = new List<(string Value, string Text, int Sort)>
            {
                ("1", "BOOKING CLIENT ADD.", 1),
                ("2", "Client Add. Master 2", 2),
                ("3", "Client Add. Master 3", 3),
                ("4", "Client Add. Master 4", 4),
                ("5", "Client Add. Master 5", 5),
            };
            return fallback;
        }
    }
}
