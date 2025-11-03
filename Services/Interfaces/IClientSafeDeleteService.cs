using System.Threading;
using System.Threading.Tasks;
using Ams.Media.Web.Dto;

namespace Ams.Media.Web.Services
{
    public interface IClientSafeDeleteService
    {
        Task<DeleteClientResult> DeleteAsync(long clientId, CancellationToken ct = default);
    }
}
