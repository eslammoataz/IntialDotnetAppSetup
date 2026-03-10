using TestingProjectSetup.Application.DTOs.Auth;
using TestingProjectSetup.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TestingProjectSetup.Api.Controllers;

/// <summary>
/// Authentication controller for OTP-based authentication
/// </summary>
public class AuthController : BaseApiController
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Generate OTP for phone number
    /// </summary>
    /// <param name="request">Phone number to send OTP to</param>
    /// <returns>OTP code (in development) or success message</returns>
    [HttpPost("otp/generate")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GenerateOtp([FromBody] GenerateOtpRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("OTP generation requested for phone: {PhoneNumber}", request.PhoneNumber);
        
        var result = await _authService.GenerateOtpAsync(request.PhoneNumber, cancellationToken);
        
        if (result.IsFailure)
        {
            return HandleResult(result);
        }

        // In development, return the OTP code. In production, just return success.
        #if DEBUG
        return Ok(new { Message = "OTP sent successfully", OtpCode = result.Value });
        #else
        return Ok(new { Message = "OTP sent successfully" });
        #endif
    }

    /// <summary>
    /// Validate OTP and get authentication token
    /// </summary>
    /// <param name="request">Phone number and OTP code</param>
    /// <returns>Authentication token and user info</returns>
    [HttpPost("otp/validate")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ValidateOtp([FromBody] ValidateOtpRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("OTP validation requested for phone: {PhoneNumber}", request.PhoneNumber);
        
        var result = await _authService.ValidateOtpAsync(request.PhoneNumber, request.OtpCode, cancellationToken);
        
        return HandleResult(result);
    }

    /// <summary>
    /// Logout and invalidate token
    /// </summary>
    /// <returns>Success or error</returns>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var result = await _authService.LogoutAsync(userId, token, cancellationToken);
        
        return HandleResult(result);
    }

    /// <summary>
    /// Refresh authentication token
    /// </summary>
    /// <returns>New authentication token</returns>
    [HttpPost("refresh")]
    [Authorize]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken(CancellationToken cancellationToken)
    {
        var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        
        var result = await _authService.RefreshTokenAsync(token, cancellationToken);
        
        return HandleResult(result);
    }

    /// <summary>
    /// Get current user info from token
    /// </summary>
    /// <returns>Current user information</returns>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var phoneNumber = User.FindFirstValue(ClaimTypes.MobilePhone);
        var name = User.FindFirstValue(ClaimTypes.Name);

        return Ok(new
        {
            UserId = userId,
            PhoneNumber = phoneNumber,
            Name = name
        });
    }
}
