using MediatR;
using Microsoft.Extensions.Logging;
using TestingProjectSetup.Application.Common;
using TestingProjectSetup.Application.DTOs.Auth;
using TestingProjectSetup.Application.Errors;
using TestingProjectSetup.Application.Interfaces;
using TestingProjectSetup.Application.Interfaces.Services;
using TestingProjectSetup.Domain.Models;

namespace TestingProjectSetup.Application.Features.Auth.Commands.LoginUser;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, Result<AuthResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly ILogger<LoginUserCommandHandler> _logger;

    public LoginUserCommandHandler(
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        ILogger<LoginUserCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<Result<AuthResponse>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var correlationId = Guid.NewGuid().ToString();

        _logger.LogInformation("Login attempt for {Email}, CorrelationId: {CorrelationId}", request.Email, correlationId);

        try
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken);

            if (user is null || !user.IsActive)
            {
                _logger.LogWarning("Invalid login attempt for email: {Email}, CorrelationId: {CorrelationId}", request.Email, correlationId);
                return Result.Failure<AuthResponse>(DomainErrors.Auth.InvalidCredentials);
            }

            var passwordValid = await _unitOfWork.Users.CheckPasswordAsync(user, request.Password);
            if (!passwordValid)
            {
                _logger.LogWarning("Invalid password attempt for email: {Email}, CorrelationId: {CorrelationId}", request.Email, correlationId);
                return Result.Failure<AuthResponse>(DomainErrors.Auth.InvalidCredentials);
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

            _logger.LogInformation("User {UserId} logged in successfully, CorrelationId: {CorrelationId}", user.Id, correlationId);

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
            _logger.LogError(ex, "Error during login for {Email}, CorrelationId: {CorrelationId}", request.Email, correlationId);
            return Result.Failure<AuthResponse>(DomainErrors.General.ServerError);
        }
    }
}
