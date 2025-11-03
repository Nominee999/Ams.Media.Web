using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Ams.Media.Web.Data
{
    /// <summary>
    /// Single source of truth สำหรับ factory เปิด SqlConnection
    /// - Create()           : คืน connection ยังไม่ Open
    /// - CreateAsync(ct)    : คืน connection ยังไม่ Open (async)
    /// - OpenAsync([ct])    : เปิดและคืน connection ที่ Open แล้ว
    /// </summary>
    public interface IDbConnectionFactory
    {
        IDbConnection Create();
        Task<IDbConnection> CreateAsync(CancellationToken ct);
        Task<IDbConnection> OpenAsync(CancellationToken ct = default);
        
    }
}
