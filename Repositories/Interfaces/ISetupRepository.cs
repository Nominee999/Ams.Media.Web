// D:\VS2022\Ams.Media.Web\Repositories\Interfaces\ISetupRepository.cs
using Ams.Media.Web.Dto;

namespace Ams.Media.Web.Repositories.Interfaces;

public interface ISetupRepository
{
    Task<IReadOnlyList<SetupTypeDto>> GetAddressTypesAsync(CancellationToken ct);
}
