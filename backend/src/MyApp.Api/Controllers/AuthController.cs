// Api/Controllers/AuthController.cs
using ExpoConnect.Contracts.Auth;
using ExpoConnect.Infrastructure.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpoConnect.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest req, CancellationToken ct)
    {
        var user = await _auth.RegisterAsync(req.Email, req.Password, req.DisplayName, ct);
        var (u, access, refresh) = await _auth.LoginAsync(req.Email, req.Password, ct);
        return Ok(new AuthResponse(access, refresh));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest req, CancellationToken ct)
    {
        var (_, access, refresh) = await _auth.LoginAsync(req.Email, req.Password, ct);
        return Ok(new AuthResponse(access, refresh));
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Refresh(RefreshRequest req, CancellationToken ct)
    {
        var (access, newRefresh) = await _auth.RefreshAsync(req.RefreshToken, ct);
        return Ok(new AuthResponse(access, newRefresh));
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(RefreshRequest req, CancellationToken ct)
    {
        await _auth.LogoutAsync(req.RefreshToken, ct);
        return NoContent();
    }
}
