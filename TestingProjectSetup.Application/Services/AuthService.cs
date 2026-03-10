using Microsoft.Extensions.Logging;
using TestingProjectSetup.Application.Common;
using TestingProjectSetup.Application.DTOs.Auth;
using TestingProjectSetup.Application.Errors;
using TestingProjectSetup.Application.Interfaces;
using TestingProjectSetup.Application.Interfaces.Services;
using TestingProjectSetup.Domain.Models;
namespace TestingProjectSetup.Application.Services;

/// <summary>
/// Authentication service implementation
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        ILogger<AuthService> logger)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<Result<string>> GenerateOtpAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber) || phoneNumber.Length < 10)
        {
            return Result.Failure<string>(DomainErrors.Auth.InvalidPhoneNumber);
        }

        try
        {
            // Invalidate existing OTPs
            await _unitOfWork.Otps.InvalidateAllOtpsForPhoneAsync(phoneNumber, cancellationToken);

            // Generate new OTP
            var otpCode = System.Security.Cryptography.RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
            var otp = new Otp
            {
                PhoneNumber = phoneNumber,
                Code = otpCode,
                Expiration = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false
            };

            await _unitOfWork.Otps.AddAsync(otp, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("OTP generated for phone: {PhoneNumber}", phoneNumber);

            return Result.Success(otpCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating OTP for {PhoneNumber}", phoneNumber);
            return Result.Failure<string>(DomainErrors.General.ServerError);
        }
    }

    public async Task<Result<AuthResponse>> ValidateOtpAsync(string phoneNumber, string otpCode, CancellationToken cancellationToken = default)
    {
        try
        {
            var otp = await _unitOfWork.Otps.GetValidOtpAsync(phoneNumber, cancellationToken);

            if (otp is null || !otp.IsValid(otpCode))
            {
                _logger.LogWarning("Invalid OTP attempt for phone: {PhoneNumber}", phoneNumber);
                return Result.Failure<AuthResponse>(DomainErrors.Auth.InvalidOtp);
            }

            // Mark OTP as used
            await _unitOfWork.Otps.InvalidateOtpAsync(otp, cancellationToken);

            // Get or create user
            var user = await _unitOfWork.Users.GetByPhoneNumberAsync(phoneNumber, cancellationToken);

            if (user is null)
            {
                user = new ApplicationUser
                {
                    PhoneNumber = phoneNumber,
                    UserName = phoneNumber,
                    Name = phoneNumber,
                    PhoneNumberConfirmed = true
                };
                await _unitOfWork.Users.AddAsync(user, cancellationToken);
            }

            // Generate token
            var token = _tokenService.GenerateToken(user);
            await _unitOfWork.Users.SaveTokenAsync(user.Id, token, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User {UserId} authenticated successfully", user.Id);

            return Result.Success(new AuthResponse(
                token,
                user.Id,
                user.PhoneNumber!,
                DateTime.UtcNow.AddHours(24)
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating OTP for {PhoneNumber}", phoneNumber);
            return Result.Failure<AuthResponse>(DomainErrors.General.ServerError);
        }
    }

    public async Task<Result> LogoutAsync(string userId, string token, CancellationToken cancellationToken = default)
    {
        try
        {
            await _unitOfWork.Users.RemoveTokenAsync(userId, token, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User {UserId} logged out", userId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout for user {UserId}", userId);
            return Result.Failure(DomainErrors.General.ServerError);
        }
    }

    public async Task<Result<AuthResponse>> RefreshTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByTokenAsync(token, cancellationToken);

            if (user is null)
            {
                return Result.Failure<AuthResponse>(DomainErrors.Auth.TokenNotFound);
            }

            // Generate new token
            var newToken = _tokenService.GenerateToken(user);
            await _unitOfWork.Users.RemoveTokenAsync(user.Id, token, cancellationToken);
            await _unitOfWork.Users.SaveTokenAsync(user.Id, newToken, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(new AuthResponse(
                newToken,
                user.Id,
                user.PhoneNumber!,
                DateTime.UtcNow.AddHours(24)
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            return Result.Failure<AuthResponse>(DomainErrors.General.ServerError);
        }
    }
}
