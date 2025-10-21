// Api/Controllers/CatalogController.cs
using ExpoConnect.Contracts.Items;
using ExpoConnect.Domain.Expo;
using ExpoConnect.Infrastructure.Items;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpoConnect.Api.Controllers;

[ApiController]
[Route("api/catalog")]
[Authorize]
public class CatalogController : ControllerBase
{
    private readonly ICatalogService _svc;
    public CatalogController(ICatalogService svc) => _svc = svc;

    [HttpGet("{standId}")]
    public async Task<ActionResult<List<CatalogItemResponse>>> ListByStand(string standId, CancellationToken ct)
    {
        var items = await _svc.ListByStandAsync(standId, ct);
        return Ok(items.Select(i =>
            new CatalogItemResponse(i.ItemId, i.StandId, i.Name, i.Description, i.Category, i.Price, i.ImageUrl, i.Features)));
    }

    [HttpGet("item/{itemId:guid}")]
    public async Task<ActionResult<CatalogItemResponse>> Get(Guid itemId, CancellationToken ct)
    {
        var i = await _svc.GetAsync(itemId, ct);
        if (i is null) return NotFound();
        return Ok(new CatalogItemResponse(i.ItemId, i.StandId, i.Name, i.Description, i.Category, i.Price, i.ImageUrl, i.Features));
    }

    [HttpPost]
    public async Task<ActionResult<CatalogItemResponse>> Create(CatalogItemCreateRequest req, CancellationToken ct)
    {
        var entity = new CatalogItem
        {
            StandId = req.StandId,
            Name = req.Name,
            Description = req.Description,
            Category = req.Category,
            Price = req.Price,
            ImageUrl = req.ImageUrl,
            Features = req.Features
        };
        var i = await _svc.CreateAsync(entity, ct);
        return CreatedAtAction(nameof(Get), new { itemId = i.ItemId },
            new CatalogItemResponse(i.ItemId, i.StandId, i.Name, i.Description, i.Category, i.Price, i.ImageUrl, i.Features));
    }

    [HttpPut("{itemId:guid}")]
    public async Task<ActionResult<CatalogItemResponse>> Update(Guid itemId, CatalogItemUpdateRequest req, CancellationToken ct)
    {
        var i = await _svc.UpdateAsync(itemId, x =>
        {
            x.Name = req.Name;
            x.Description = req.Description;
            x.Category = req.Category;
            x.Price = req.Price;
            x.ImageUrl = req.ImageUrl;
            x.Features = req.Features;
        }, ct);

        if (i is null) return NotFound();
        return Ok(new CatalogItemResponse(i.ItemId, i.StandId, i.Name, i.Description, i.Category, i.Price, i.ImageUrl, i.Features));
    }

    [HttpDelete("{itemId:guid}")]
    public async Task<IActionResult> Delete(Guid itemId, CancellationToken ct)
    {
        var ok = await _svc.DeleteAsync(itemId, ct);
        return ok ? NoContent() : NotFound();
    }
}
