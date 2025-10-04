// Controllers/Api/SetupController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace Ams.Media.Web.Controllers.Api;

[ApiController]
[Route("api/setup")]
public sealed class SetupController : ControllerBase
{
    private readonly string _cs;
    public SetupController(IConfiguration config) => _cs = config.GetConnectionString("Default")!;

    public sealed class AddressTypeDto
    {
        public int code { get; set; }
        public string name { get; set; } = "";
    }

    [HttpGet("address-types")]
    public async Task<IActionResult> GetAddressTypes(CancellationToken ct)
    {
        var candidateTables = new[]
        {
            ("dbo","AddressType"),
            ("dbo","AddressTypes"),
            ("dbo","MasAddressType"),
            ("dbo","Address_Type"),
        };

        await using var con = new SqlConnection(_cs);
        await con.OpenAsync(ct);

        // หา table ที่มีอยู่จริงก่อน
        (string schema, string table)? found = null;
        foreach (var (schema, table) in candidateTables)
        {
            const string existsSql = @"
                SELECT 1
                FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_SCHEMA=@schema AND TABLE_NAME=@table;";
            await using var existsCmd = new SqlCommand(existsSql, con);
            existsCmd.Parameters.AddWithValue("@schema", schema);
            existsCmd.Parameters.AddWithValue("@table", table);
            var has = await existsCmd.ExecuteScalarAsync(ct);
            if (has is not null)
            {
                found = (schema, table);
                break;
            }
        }

        var items = new List<AddressTypeDto>();

        if (found is not null)
        {
            // เช็คว่ามีคอลัมน์ชื่อ AddressTypeName ไหม
            const string colsSql = @"
                SELECT COLUMN_NAME
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_SCHEMA=@schema AND TABLE_NAME=@table;";
            await using var cols = new SqlCommand(colsSql, con);
            cols.Parameters.AddWithValue("@schema", found.Value.schema);
            cols.Parameters.AddWithValue("@table", found.Value.table);
            var colsSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            await using (var rdrCols = await cols.ExecuteReaderAsync(ct))
            {
                while (await rdrCols.ReadAsync(ct))
                    colsSet.Add(rdrCols.GetString(0));
            }

            var codeCol = colsSet.Contains("AddressType") ? "AddressType"
                        : colsSet.Contains("Code") ? "Code"
                        : colsSet.Contains("Type") ? "Type"
                        : null;

            if (codeCol is null)
                return Problem($"Cannot resolve code column in {found.Value.schema}.{found.Value.table}");

            string? nameCol = null;
            foreach (var cand in new[] { "AddressTypeName", "Name", "TypeName", "Description" })
                if (colsSet.Contains(cand)) { nameCol = cand; break; }

            var sql = nameCol is not null
                ? $"SELECT CAST([{codeCol}] AS int) AS code, CAST([{nameCol}] AS nvarchar(255)) AS name FROM [{found.Value.schema}].[{found.Value.table}] ORDER BY [{codeCol}];"
                : $"SELECT CAST([{codeCol}] AS int) AS code, CAST([{codeCol}] AS nvarchar(255)) AS name FROM [{found.Value.schema}].[{found.Value.table}] ORDER BY [{codeCol}];";

            await using var cmd = new SqlCommand(sql, con);
            await using var rdr = await cmd.ExecuteReaderAsync(ct);
            while (await rdr.ReadAsync(ct))
            {
                items.Add(new AddressTypeDto
                {
                    code = rdr.GetInt32(0),
                    name = rdr.IsDBNull(1) ? "" : rdr.GetString(1)
                });
            }
        }
        else
        {
            // ไม่มีตาราง lookup เลย → fallback จาก Address จริง
            // ต้องมีตาราง dbo.Address และคอลัมน์ AddressType (int)
            const string existsAddr = @"
                SELECT 1
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='Address' AND COLUMN_NAME='AddressType';";
            await using var chk = new SqlCommand(existsAddr, con);
            var hasAddrType = await chk.ExecuteScalarAsync(ct) is not null;
            if (!hasAddrType)
                return Problem("Address types lookup not found and dbo.Address.AddressType column is missing.");

            const string distinctSql = @"
                SELECT DISTINCT CAST(AddressType AS int) AS code
                FROM dbo.Address
                WHERE AddressType IS NOT NULL
                ORDER BY code;";
            await using var cmd = new SqlCommand(distinctSql, con);
            await using var rdr = await cmd.ExecuteReaderAsync(ct);
            while (await rdr.ReadAsync(ct))
            {
                var code = rdr.GetInt32(0);
                items.Add(new AddressTypeDto { code = code, name = code.ToString() });
            }
        }

        return Ok(items);
    }
}
