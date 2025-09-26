using Ams.Media.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ams.Media.Web.Controllers.Api;

[ApiController]
[Route("api/clients")]
public sealed class ClientsController(IClientService clientService) : ControllerBase
{
    [HttpGet("{clientCode:int}/usage")]
    public async Task<IActionResult> GetUsage(int clientCode, CancellationToken ct)
        => Ok(await clientService.GetUsageAsync(clientCode, ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Dtos.ClientDto dto, CancellationToken ct)
    {
        var id = await clientService.SaveAsync(dto, ct);
        return CreatedAtAction(nameof(GetUsage), new { clientCode = id }, new { clientCode = id });
    }

    [HttpPut("{clientCode:int}")]
    public async Task<IActionResult> Update(int clientCode, [FromBody] Dtos.ClientDto dto, CancellationToken ct)
    {
        var id = await clientService.UpdateAsync(clientCode, dto, ct);
        return Ok(new { clientCode = id });
    }

    [HttpDelete("{clientCode:int}")]
    public async Task<IActionResult> Delete(int clientCode, CancellationToken ct)
    {
        var usage = await clientService.GetUsageAsync(clientCode, ct);
        var inUse = usage.Where(x => x.Count > 0).ToList();

        if (inUse.Count > 0)
        {
            return Conflict(new
            {
                message = "ยังมีการใช้งานอยู่ ไม่สามารถลบได้",
                clientCode,
                usedBy = inUse
            });
        }

        var ok = await clientService.DeleteSafeAsync(clientCode, ct);
        if (!ok)
        {
            return Conflict(new
            {
                message = "ยังมีการใช้งานอยู่ (DB ตรวจพบ)",
                clientCode
            });
        }

        return NoContent();
    }


}
