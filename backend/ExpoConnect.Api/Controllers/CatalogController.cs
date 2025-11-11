using ExpoConnect.Application.Interfaces;
using ExpoConnect.Contracts.Catalogs;
using ExpoConnect.Contracts.Items;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpoConnect.Api.Controllers;

[ApiController]
[Route("api/catalogs")]
[Authorize]
public class CatalogController(ICatalogService svc) : ControllerBase
{
    private readonly ICatalogService _svc = svc;

    [HttpGet]
    public async Task<ActionResult<List<CatalogResponse>>> List(CancellationToken ct)
        => Ok(await _svc.ListAsync(ct));

    [HttpGet("{catalogId:guid}")]
    public async Task<ActionResult<CatalogResponse>> Get(Guid catalogId, CancellationToken ct)
    {
        var result = await _svc.GetAsync(catalogId, ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<CatalogResponse>> Create([FromBody] CreateCatalogRequest req, CancellationToken ct)
        => Ok(await _svc.CreateAsync(req, ct));

    [HttpPost("items")]
    public async Task<ActionResult<CatalogItemResponse>> AddItem([FromBody] CreateCatalogItemRequest req, CancellationToken ct)
        => Ok(await _svc.AddItemAsync(req, ct));
}
