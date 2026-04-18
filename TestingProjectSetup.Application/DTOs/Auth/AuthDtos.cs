namespace TestingProjectSetup.Application.DTOs.Auth;

public record RegisterRequest(string Name, string Email, string PhoneNumber, string Password);

public record LoginRequest(string Email, string Password);

public record RefreshTokenRequest(string RefreshToken);

public record RevokeTokenRequest(string RefreshToken);

public record ChangePasswordRequest(string UserId, string CurrentPassword, string NewPassword);

public record AuthResponse(
    string? Token,
    string? RefreshToken,
    string UserId,
    string Email,
    string Name,
    string? Role,
    DateTime? ExpiresAt,
    string? PhoneNumber);

public record LogoutRequest(string? RefreshToken);
