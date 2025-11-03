using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ams.Media.Web.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ams.Media.Web.Services
{
    public sealed class SetupService : ISetupService
    {
        private readonly ISetupRepository _repo;
        private const string ADDRESS_TYPE_KEY = "CLIENT_ADDRESS_TYPE"; // เปลี่ยนได้จุดเดียว

        public SetupService(ISetupRepository repo) => _repo = repo;

        public async Task<IReadOnlyList<SelectListItem>> GetOptionsAsync(string setupTypeKey, CancellationToken ct)
        {
            var rows = await _repo.GetOptionsAsync(setupTypeKey, ct);
            return rows.OrderBy(r => r.Sort)
                       .Select(r => new SelectListItem { Value = r.Value, Text = r.Text })
                       .ToList();
        }

        public Task<IReadOnlyList<SelectListItem>> GetAddressTypesAsync(CancellationToken ct)
            => GetOptionsAsync(ADDRESS_TYPE_KEY, ct);
    }
}
