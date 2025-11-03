using System.Threading;
using System.Threading.Tasks;
using Ams.Media.Web.Dto;

namespace Ams.Media.Web.Repositories.Interfaces
{
    public interface IClientRepository
    {
        // Grid
        Task<PagedResult<ClientGridRow>> ListPagedAsync(string? q, int page, int pageSize, string show, CancellationToken ct);

        // CRUD ที่ ClientService และ ClientController ใช้
        Task<long> GetNextClientIdAsync();
        Task<ClientRow?> GetAsync(long id);
        Task<bool> CreateAsync(ClientRow input);
        Task<bool> UpdateAsync(ClientRow input);
        Task<bool> DeleteAsync(long id);
    }
}
