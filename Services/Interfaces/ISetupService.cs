// D:\VS2022\Ams.Media.Web\Services\Interfaces\ISetupService.cs
using Ams.Media.Web.Dto;

namespace Ams.Media.Web.Services.Interfaces;

public interface ISetupService
{
    Task<IReadOnlyList<SetupTypeDto>> GetAddressTypesAsync(CancellationToken ct);
}
