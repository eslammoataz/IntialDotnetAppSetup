namespace TestingProjectSetup.Application.DTOs.Auth;

public record GenerateOtpRequest(string PhoneNumber);

public record ValidateOtpRequest(string PhoneNumber, string OtpCode);

public record AuthResponse(string Token, string UserId, string PhoneNumber, DateTime ExpiresAt);

public record LogoutRequest(string Token);
