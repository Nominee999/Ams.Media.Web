using System.Data;
using Dapper;
using Ams.Media.Web.Data;
using Ams.Media.Web.Repositories.Interfaces;
using Ams.Media.Web.Dto;

namespace Ams.Media.Web.Repositories
{
    public sealed class SetupRepository : ISetupRepository
    {
        private readonly DbConnectionFactory _db;
        public SetupRepository(DbConnectionFactory db) => _db = db;

        public async Task<IReadOnlyList<SetupTypeDto>> GetAddressTypesAsync(CancellationToken ct)
        {
            using var con = await _db.OpenAsync(ct); // ✅ ใช้ overload ที่รองรับ ct
            const string sql = @"
SELECT SetuptypeCode AS Code, SetuptypeName AS Name
FROM   SetupType WITH (NOLOCK)
WHERE  SetuptypeCode IN (1,2,3,4,5)
ORDER BY SetuptypeCode;";
            var rows = await con.QueryAsync<SetupTypeDto>(sql);
            return rows.ToList();
        }
    }
}
