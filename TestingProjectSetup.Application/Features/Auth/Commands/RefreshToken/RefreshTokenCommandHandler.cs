using MediatR;
using Microsoft.Extensions.Logging;
using TestingProjectSetup.Application.Common;
using TestingProjectSetup.Application.DTOs.Auth;
using TestingProjectSetup.Application.Errors;
using TestingProjectSetup.Application.Interfaces;
using TestingProjectSetup.Application.Interfaces.Services;

namespace TestingProjectSetup.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;

    public RefreshTokenCommandHandler(
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        ILogger<RefreshTokenCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var tokenHash = _tokenService.HashToken(request.RefreshToken);
        var refreshToken = await _unitOfWork.RefreshTokens.GetByTokenHashAsync(tokenHash, cancellationToken);

        if (refreshToken is null)
        {
            _logger.LogWarning("Invalid refresh token attempted");
            return Result.Failure<AuthResponse>(DomainErrors.Auth.InvalidRefreshToken);
        }

        if (refreshToken.RevokedAt != null)
        {
            _logger.LogWarning("Suspicious activity: revoked refresh token reused for user {UserId}. Revoking all tokens.", refreshToken.UserId);
            await _unitOfWork.RefreshTokens.RevokeAllTokensForUserAsync(refreshToken.UserId, "Token reuse detected", request.IpAddress, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Failure<AuthResponse>(DomainErrors.Auth.RefreshTokenReused);
        }

        if (refreshToken.IsExpired)
        {
            return Result.Failure<AuthResponse>(DomainErrors.Auth.RefreshTokenExpired);
        }

        var user = refreshToken.User;
        if (user is null || !user.IsActive)
        {
            return Result.Failure<AuthResponse>(DomainErrors.Auth.AccountInactive);
        }

        var roles = await _unitOfWork.Users.GetRolesAsync(user);
        var (accessToken, expiresAt) = _tokenService.GenerateTokenWithExpiry(user, roles);

        var newRefreshToken = _tokenService.GenerateRefreshToken();
        var newRefreshTokenHash = _tokenService.HashToken(newRefreshToken);

        refreshToken.RevokedAt = DateTime.UtcNow;
        refreshToken.RevokedByIp = request.IpAddress;
        refreshToken.ReplacedByTokenHash = newRefreshTokenHash;

        await _unitOfWork.RefreshTokens.AddAsync(new Domain.Models.RefreshToken
        {
            UserId = user.Id,
            TokenHash = newRefreshTokenHash,
            ExpiresAt = DateTime.UtcNow.AddDays(_tokenService.RefreshTokenExpirationInDays),
            CreatedByIp = request.IpAddress,
            User = null!
        }, cancellationToken);

        await _unitOfWork.RefreshTokens.UpdateAsync(refreshToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Refresh token rotated for user {UserId}", user.Id);

        return Result.Success(new AuthResponse(
            Token: accessToken,
            RefreshToken: newRefreshToken,
            UserId: user.Id,
            Email: user.Email!,
            Name: user.Name ?? "",
            Role: roles.FirstOrDefault(),
            ExpiresAt: expiresAt,
            PhoneNumber: user.PhoneNumber
        ));
    }
}
