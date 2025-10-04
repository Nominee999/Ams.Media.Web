using System.Threading;
using System.Threading.Tasks;
using Ams.Media.Web.Dto;

namespace Ams.Media.Web.Repositories.Interfaces
{
    public interface IClientRepository
    {
        // ลิสต์หน้าแบบมี paging
        Task<PagedResult<ClientRow>> ListAsync(string? q, int page, int pageSize, string showMode, CancellationToken ct);

        // ===== Client CRUD (ที่ Service เรียกใช้อยู่) =====
        Task<ClientRow?> GetAsync(int clientId, CancellationToken ct);
        Task<int> GetNextClientCodeAsync(CancellationToken ct);
        Task<(bool ok, string? message, ClientRow? saved)> SaveAsync(ClientRow dto, CancellationToken ct);
        Task<(bool ok, string? reason)> SafeDeleteAsync(int clientId, CancellationToken ct);
    }
}
