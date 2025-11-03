using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Ams.Media.Web.Data
{
    /// <summary>
    /// Implement ตาม IDbConnectionFactory เดียวของโปรเจกต์
    /// </summary>
    public sealed class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;
        public DbConnectionFactory(string connectionString)
        {
            _connectionString = connectionString ?? "";
        }


        public Task<IDbConnection> CreateAsync(CancellationToken ct)
            => Task.FromResult<IDbConnection>(new SqlConnection(_connectionString));

        public async Task<IDbConnection> OpenAsync(CancellationToken ct = default)
        {
            var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync(ct);
            return conn;
        }

        public IDbConnection Create()
        {
            var cn = new SqlConnection(_connectionString);
            // ไม่ open ที่นี่ ปล่อยให้ Dapper เปิดเองตามการใช้งาน
            return cn;
        }
    }

}

