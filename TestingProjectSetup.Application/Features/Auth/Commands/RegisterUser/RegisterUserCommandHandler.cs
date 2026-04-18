using MediatR;
using Microsoft.Extensions.Logging;
using TestingProjectSetup.Application.Common;
using TestingProjectSetup.Application.DTOs.Auth;
using TestingProjectSetup.Application.Errors;
using TestingProjectSetup.Application.Interfaces;
using TestingProjectSetup.Application.Interfaces.Services;
using TestingProjectSetup.Domain.Models;

namespace TestingProjectSetup.Application.Features.Auth.Commands.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<AuthResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly ILogger<RegisterUserCommandHandler> _logger;

    public RegisterUserCommandHandler(
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        ILogger<RegisterUserCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<Result<AuthResponse>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var correlationId = Guid.NewGuid().ToString();

        _logger.LogInformation(
            "RegisterUser request started. Email: {Email}, CorrelationId: {CorrelationId}",
            request.Email, correlationId);

        try
        {
            var existingUser = await _unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken);
            if (existingUser != null)
            {
                _logger.LogWarning(
                    "Registration failed: Email already exists. Email: {Email}, CorrelationId: {CorrelationId}",
                    request.Email, correlationId);
                return Result.Failure<AuthResponse>(DomainErrors.User.AlreadyExists);
            }

            var user = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.Email,
                Name = request.Name,
                PhoneNumber = request.PhoneNumber,
                EmailConfirmed = true,
                IsActive = true
            };

            var (success, errors) = await _unitOfWork.Users.CreateUserWithResultAsync(user, request.Password, cancellationToken);

            if (!success)
            {
                var errorMessages = errors.Select(e => e.Description).ToList();
                _logger.LogWarning(
                    "Registration failed: Identity error. Email: {Email}, Errors: {Errors}, CorrelationId: {CorrelationId}",
                    request.Email, string.Join("; ", errorMessages), correlationId);
                return Result.Failure<AuthResponse>(new Error("Validation.IdentityErrors", "One or more validation errors occurred.") { Errors = errorMessages });
            }

            var roles = await _unitOfWork.Users.GetRolesAsync(user);
            var (token, expiresAt) = _tokenService.GenerateTokenWithExpiry(user, roles);

            var refreshToken = _tokenService.GenerateRefreshToken();
            var refreshTokenHash = _tokenService.HashToken(refreshToken);

            await _unitOfWork.RefreshTokens.AddAsync(new Domain.Models.RefreshToken
            {
                UserId = user.Id,
                TokenHash = refreshTokenHash,
                ExpiresAt = DateTime.UtcNow.AddDays(_tokenService.RefreshTokenExpirationInDays),
                User = null!
            }, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "User {UserId} registered successfully. Email: {Email}, CorrelationId: {CorrelationId}",
                user.Id, request.Email, correlationId);

            return Result.Success(new AuthResponse(
                Token: token,
                RefreshToken: refreshToken,
                UserId: user.Id,
                Email: user.Email!,
                Name: user.Name ?? "",
                Role: roles.FirstOrDefault(),
                ExpiresAt: expiresAt,
                PhoneNumber: user.PhoneNumber
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unhandled exception during user registration. Email: {Email}, CorrelationId: {CorrelationId}",
                request.Email, correlationId);
            return Result.Failure<AuthResponse>(DomainErrors.General.ServerError);
        }
    }
}
