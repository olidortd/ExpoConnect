using ExpoConnect.Application.Interfaces;
using ExpoConnect.Contracts.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin")]
public class UsersController : ControllerBase
{
    private readonly IUsersService _svc;
    public UsersController(IUsersService svc) => _svc = svc;

    // GET /api/users?q=&page=1&pageSize=20&sort=-created
    [HttpGet]
    public async Task<ActionResult<PagedResult<UserResponse>>> Query(
        [FromQuery] string? q, [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20, [FromQuery] string? sort = "-created",
        CancellationToken ct = default)
        => Ok(await _svc.QueryAsync(q, Math.Max(1, page), Math.Clamp(pageSize, 1, 200), sort, ct));

    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponse>> GetById(string id, CancellationToken ct)
    {
        var user = await _svc.GetAsync(id, ct);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<UserResponse>> Create([FromBody] UserCreateRequest req, CancellationToken ct)
    {
        var created = await _svc.CreateAsync(req, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.UserId }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UserResponse>> Update(string id, [FromBody] UserUpdateRequest req, CancellationToken ct)
        => Ok(await _svc.UpdateAsync(id, req, ct));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        await _svc.DeleteAsync(id, ct);
        return NoContent();
    }
}
