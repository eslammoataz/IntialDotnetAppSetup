using MediatR;
using TestingProjectSetup.Application.Common;

namespace TestingProjectSetup.Application.Features.Auth.Commands.RevokeAllTokens;

public record RevokeAllTokensCommand(string UserId, string? IpAddress = null) : IRequest<Result>;
