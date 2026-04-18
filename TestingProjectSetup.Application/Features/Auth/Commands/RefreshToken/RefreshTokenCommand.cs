using MediatR;
using TestingProjectSetup.Application.Common;
using TestingProjectSetup.Application.DTOs.Auth;

namespace TestingProjectSetup.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string RefreshToken, string? IpAddress = null) : IRequest<Result<AuthResponse>>;
