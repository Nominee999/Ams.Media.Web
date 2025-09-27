using Ams.Media.Web.Services;
using Ams.Media.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace Ams.Media.Web.Controllers.Api;

[ApiController]
[Produces("application/json")]
[Route("api/clients")]
public sealed class ClientsController(IClientService svc, IHostEnvironment env) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int page, [FromQuery] int pageSize, [FromQuery] string? q, CancellationToken ct)
    {
        if (page <= 0 || pageSize <= 0)
            return BadRequest(new { message = "page และ pageSize ต้องมากกว่า 0", page, pageSize });

        try
        {
            var (items, total) = await svc.ListAsync(q, page, pageSize, ct);
            return Ok(new { items, total, page, pageSize, q });
        }
        catch (System.Exception ex)
        {
            if (env.IsDevelopment())
            {
                return Problem(
                    title: ex.GetType().FullName,
                    detail: ex.ToString(),
                    statusCode: 500
                );
            }
            throw;
        }
    }
}
