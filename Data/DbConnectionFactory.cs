using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace Ams.Media.Web.Data
{
    public sealed class DbConnectionFactory
    {
        private readonly string _connString;

        // ใช้จาก DI
        public DbConnectionFactory(IConfiguration config)
        {
            _connString = config.GetConnectionString("AmsDb")!;
        }

        // ใช้ new ด้วย string โดยตรง
        public DbConnectionFactory(string connectionString)
        {
            _connString = connectionString;
        }

        public IDbConnection Create() => new SqlConnection(_connString);

        // เดิมไม่มี ct → คงไว้
        public async Task<IDbConnection> OpenAsync()
        {
            var conn = new SqlConnection(_connString);
            await conn.OpenAsync().ConfigureAwait(false);
            return conn;
        }

        // เพิ่ม overload รองรับ CancellationToken (ให้โค้ดที่เรียก OpenAsync(ct) คอมไพล์ได้)
        public async Task<IDbConnection> OpenAsync(CancellationToken ct)
        {
            var conn = new SqlConnection(_connString);
            await conn.OpenAsync(ct).ConfigureAwait(false);
            return conn;
        }
    }
}
