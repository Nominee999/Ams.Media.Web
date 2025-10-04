// D:\VS2022\Ams.Media.Web\Services\SetupService.cs
using Ams.Media.Web.Dto;
using Ams.Media.Web.Repositories.Interfaces;
using Ams.Media.Web.Services.Interfaces;

namespace Ams.Media.Web.Services;

public sealed class SetupService : ISetupService
{
    private readonly ISetupRepository _repo;
    public SetupService(ISetupRepository repo) => _repo = repo;

    public Task<IReadOnlyList<SetupTypeDto>> GetAddressTypesAsync(CancellationToken ct)
        => _repo.GetAddressTypesAsync(ct);
}
