using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ams.Media.Web.Services
{
    public interface ISetupService
    {
        Task<IReadOnlyList<SelectListItem>> GetOptionsAsync(string setupTypeKey, CancellationToken ct);
        Task<IReadOnlyList<SelectListItem>> GetAddressTypesAsync(CancellationToken ct);
    }
}
