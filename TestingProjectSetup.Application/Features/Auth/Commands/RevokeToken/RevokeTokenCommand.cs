using MediatR;
using TestingProjectSetup.Application.Common;

namespace TestingProjectSetup.Application.Features.Auth.Commands.RevokeToken;

public record RevokeTokenCommand(string RefreshToken, string? IpAddress = null) : IRequest<Result>;
