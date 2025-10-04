using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Ams.Media.Web.Data;
using Ams.Media.Web.Dto;
using Ams.Media.Web.Repositories.Interfaces;

namespace Ams.Media.Web.Repositories
{
    public sealed class ClientAddressRepository : IClientAddressRepository
    {
        private readonly DbConnectionFactory _db;
        public ClientAddressRepository(DbConnectionFactory db) => _db = db;

        public async Task<IReadOnlyList<ClientAddressDto>> ListAsync(int clientId, int? type, CancellationToken ct)
        {
            using var con = await _db.OpenAsync(ct);
            const string sql = @"
SELECT  ClientID      AS ClientId,
        AddressType,
        StartDate,
        EndDate,
        CompanyName,
        AddressName,
        AddressTitle,
        MultiAddress01,
        MultiAddress02,
        MultiAddress03,
        MultiAddress04,
        MultiAreaCode,
        ZipCode,
        MultiStateCode,
        MultiCountry,
        MultiComments
FROM dbo.Address WITH (NOLOCK)
WHERE ClientCode = @clientId
  AND (@type IS NULL OR AddressType = @type)
ORDER BY AddressType, StartDate;";
            var rows = await con.QueryAsync<ClientAddressDto>(sql, new { clientId, type }, commandType: CommandType.Text);
            return rows.ToList();
        }

        public async Task<bool> ExistsOverlapAsync(int clientId, int addressType, DateTime start, DateTime end,
                                                   DateTime? excludeStart, DateTime? excludeEnd, CancellationToken ct)
        {
            using var con = await _db.OpenAsync(ct);
            const string sql = @"
SELECT COUNT(1)
FROM dbo.Address WITH (NOLOCK)
WHERE ClientCode = @clientId
  AND AddressType = @addressType
  AND NOT (EndDate < @start OR StartDate > @end)
  AND NOT (@excludeStart IS NOT NULL AND @excludeEnd IS NOT NULL
           AND StartDate = @excludeStart AND EndDate = @excludeEnd);";
            var n = await con.ExecuteScalarAsync<int>(sql, new
            {
                clientId,
                addressType,
                start,
                end,
                excludeStart,
                excludeEnd
            });
            return n > 0;
        }

        public async Task UpsertAsync(ClientAddressDto dto, DateTime? oldStart, DateTime? oldEnd, CancellationToken ct)
        {
            using var con = await _db.OpenAsync(ct);
            using var tx = con.BeginTransaction();

            // ถ้ามี oldStart/oldEnd แปลว่า update → ลบแถวเดิมก่อน
            if (oldStart.HasValue && oldEnd.HasValue)
            {
                const string del = @"
DELETE FROM dbo.Address
WHERE ClientCode = @clientId AND AddressType = @addressType
  AND StartDate = @oldStart AND EndDate = @oldEnd;";
                await con.ExecuteAsync(del, new
                {
                    clientId = dto.ClientId,
                    addressType = dto.AddressType,
                    oldStart,
                    oldEnd
                }, tx);
            }

            const string ins = @"
INSERT INTO dbo.Address(
    ClientCode, AddressType, StartDate, EndDate,
    CompanyName, AddressName, AddressTitle,
    MultiAddress01, MultiAddress02, MultiAddress03, MultiAddress04,
    MultiAreaCode, ZipCode, MultiStateCode, MultiCountry, MultiComments)
VALUES(
    @ClientId, @AddressType, @StartDate, @EndDate,
    @CompanyName, @AddressName, @AddressTitle,
    @MultiAddress01, @MultiAddress02, @MultiAddress03, @MultiAddress04,
    @MultiAreaCode, @ZipCode, @MultiStateCode, @MultiCountry, @MultiComments);";

            await con.ExecuteAsync(ins, dto, tx);
            tx.Commit();
        }

        public async Task<bool> DeleteAsync(int clientId, int addressType, DateTime start, DateTime end, CancellationToken ct)
        {
            using var con = await _db.OpenAsync(ct);
            var rows = await con.ExecuteAsync(@"
DELETE FROM dbo.Address
WHERE ClientCode=@clientId AND AddressType=@addressType AND StartDate=@start AND EndDate=@end;",
                new { clientId, addressType, start, end });
            return rows > 0;
        }
    }
}
