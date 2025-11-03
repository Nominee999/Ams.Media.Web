using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ams.Media.Web.Repositories.Interfaces
{
    public interface ISetupRepository
    {
        Task<IReadOnlyList<(string Value, string Text, int Sort)>> GetOptionsAsync(string setupTypeKey, CancellationToken ct);
    }
}
