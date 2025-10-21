// Contracts/Auth/AuthDtos.cs
namespace ExpoConnect.Contracts.Auth;

public record RegisterRequest(string Email, string Password, string? DisplayName);
public record LoginRequest(string Email, string Password);
public record AuthResponse(string AccessToken, string RefreshToken, string TokenType = "Bearer");
public record RefreshRequest(string RefreshToken);
