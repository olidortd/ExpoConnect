using ExpoConnect.Application.Interfaces;
using ExpoConnect.Contracts.Stands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpoConnect.Api.Controllers;

[ApiController]
[Route("api/stands")]
[Authorize]
public class StandsController(IStandsService svc) : ControllerBase
{
    private readonly IStandsService _svc = svc;

    [HttpPost]
    public async Task<ActionResult<StandResponse>> Create([FromBody] CreateStandRequest req, CancellationToken ct)
        => Ok(await _svc.CreateAsync(req, ct));
}
