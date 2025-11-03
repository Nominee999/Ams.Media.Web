// Repositories/ClientAddressRepository.cs
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Ams.Media.Web.Dto;
using Ams.Media.Web.Repositories.Interfaces;

namespace Ams.Media.Web.Repositories
{
    public sealed class ClientAddressRepository : IClientAddressRepository
    {
        private readonly string _connStr;

        public ClientAddressRepository(IConfiguration cfg)
        {
            _connStr = cfg.GetConnectionString("AmsDb")
                       ?? cfg.GetConnectionString("Default")
                       ?? "";
        }

        public async Task<IReadOnlyList<ClientAddressDto>> ListAsync(int clientId, int? addressType, CancellationToken ct)
        {
            const string sql = @"
SELECT
    ClientID     AS ClientId,
    AddressType,
    StartDate,
    EndDate,
    AddressTitle,
    Address01,
    Address02,
    Address03,
    Address04,
    AddressTitle AS AddressName
FROM dbo.Address WITH (NOLOCK)
WHERE ClientID = @clientId
  AND (@addressType IS NULL OR AddressType = @addressType)
ORDER BY AddressType, StartDate;";

            await using var conn = new SqlConnection(_connStr);
            var rows = await conn.QueryAsync<ClientAddressDto>(
                new CommandDefinition(sql, new { clientId, addressType }, cancellationToken: ct));
            return rows.AsList();
        }

        public async Task<ClientAddressDto?> GetAsync(int clientId, int addressType, DateTime start, DateTime? end, CancellationToken ct)
        {
            const string sql = @"
SELECT
    ClientID     AS ClientId,
    AddressType,
    StartDate,
    EndDate,
    AddressTitle,
    Address01,
    Address02,
    Address03,
    Address04,
    AddressTitle AS AddressName
FROM dbo.Address WITH (NOLOCK)
WHERE ClientID    = @clientId
  AND AddressType = @addressType
  AND StartDate   = @start;";

            await using var conn = new SqlConnection(_connStr);
            return await conn.QueryFirstOrDefaultAsync<ClientAddressDto>(
                new CommandDefinition(sql, new { clientId, addressType, start }, cancellationToken: ct));
        }

        public async Task<bool> IsOverlapAsync(
            int clientId,
            int addressType,
            DateTime start,
            DateTime? end,
            (int clientId, int addressType, DateTime startDate)? oldKey,
            CancellationToken ct)
        {
            // ใช้ช่วง [StartDate..EndDate] แบบ inclusive; ถ้า EndDate null ให้ใช้ 9999-12-31
            var newEnd = end ?? new DateTime(9999, 12, 31);

            const string sql = @"
SELECT TOP 1 1
FROM dbo.Address WITH (NOLOCK)
WHERE ClientID    = @clientId
  AND AddressType = @addressType
  AND NOT ( @newEnd < StartDate OR @newStart > EndDate )
  AND NOT (
        @oldStart IS NOT NULL
        AND ClientID    = @oldClient
        AND AddressType = @oldType
        AND StartDate   = @oldStart
  );";

            await using var conn = new SqlConnection(_connStr);
            var exists = await conn.ExecuteScalarAsync<int?>(
                new CommandDefinition(
                    sql,
                    new
                    {
                        clientId,
                        addressType,
                        newStart = start,
                        newEnd,
                        oldStart = oldKey?.startDate,
                        oldType = oldKey?.addressType,
                        oldClient = oldKey?.clientId
                    },
                    cancellationToken: ct));

            return exists.HasValue;
        }

        public async Task<bool> CreateAsync(ClientAddressDto dto, CancellationToken ct)
        {
            // กันซ้อนช่วงวัน
            var overlap = await IsOverlapAsync(dto.ClientId, dto.AddressType, dto.StartDate, dto.EndDate, null, ct);
            if (overlap) return false;

            const string sql = @"
INSERT INTO dbo.Address
( ClientID, AddressType, StartDate, EndDate, AddressTitle, Address01, Address02, Address03, Address04 )
VALUES
( @ClientId, @AddressType, @StartDate, @EndDate, @AddressTitle, @Address01, @Address02, @Address03, @Address04 );";

            await using var conn = new SqlConnection(_connStr);
            var n = await conn.ExecuteAsync(new CommandDefinition(sql, dto, cancellationToken: ct));
            return n > 0;
        }

        public async Task<bool> UpdateAsync(int clientId, int addressType, DateTime oldStart, ClientAddressDto dto, CancellationToken ct)
        {
            // กันซ้อนช่วงวัน (ยกเว้นตนเอง)
            var overlap = await IsOverlapAsync(clientId, addressType, dto.StartDate, dto.EndDate,
                                               (clientId, addressType, oldStart), ct);
            if (overlap) return false;

            const string sql = @"
UPDATE dbo.Address
SET
    StartDate    = @StartDate,
    EndDate      = @EndDate,
    AddressTitle = @AddressTitle,
    Address01    = @Address01,
    Address02    = @Address02,
    Address03    = @Address03,
    Address04    = @Address04
WHERE ClientID    = @clientId
  AND AddressType = @addressType
  AND StartDate   = @oldStart;";

            await using var conn = new SqlConnection(_connStr);
            var n = await conn.ExecuteAsync(new CommandDefinition(sql, new
            {
                clientId,
                addressType,
                oldStart,
                dto.StartDate,
                dto.EndDate,
                dto.AddressTitle,
                dto.Address01,
                dto.Address02,
                dto.Address03,
                dto.Address04
            }, cancellationToken: ct));
            return n > 0;
        }

        public async Task<bool> DeleteAsync(int clientId, int addressType, DateTime start, DateTime? end, CancellationToken ct)
        {
            const string sql = @"
DELETE FROM dbo.Address
WHERE ClientID    = @clientId
  AND AddressType = @addressType
  AND StartDate   = @start;";

            await using var conn = new SqlConnection(_connStr);
            var n = await conn.ExecuteAsync(new CommandDefinition(sql, new { clientId, addressType, start }, cancellationToken: ct));
            return n > 0;
        }

        public async Task<bool> UpsertAsync(ClientAddressDto dto, DateTime? oldStart, DateTime? oldEnd, CancellationToken ct)
        {
            if (oldStart.HasValue)
                return await UpdateAsync(dto.ClientId, dto.AddressType, oldStart.Value, dto, ct);
            return await CreateAsync(dto, ct);
        }
    }
}
