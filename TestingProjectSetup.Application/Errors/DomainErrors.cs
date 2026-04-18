using TestingProjectSetup.Application.Common;

namespace TestingProjectSetup.Application.Errors;

/// <summary>
/// Domain-specific errors
/// </summary>
public static class DomainErrors
{
    public static class Auth
    {
        public static readonly Error InvalidCredentials = new(
            "Auth.InvalidCredentials", "Invalid email or password.");

        public static readonly Error UserNotFound = new(
            "Auth.UserNotFound", "User was not found.");

        public static readonly Error TokenNotFound = new(
            "Auth.TokenNotFound", "Authentication token was not found.");

        public static readonly Error TokenExpired = new(
            "Auth.TokenExpired", "Authentication token has expired.");

        public static readonly Error Unauthorized = new(
            "Auth.Unauthorized", "You are not authorized to perform this action.");

        public static readonly Error AccountInactive = new(
            "Auth.AccountInactive", "Your account is inactive. Please contact HR.");

        public static readonly Error InvalidRefreshToken = new(
            "Auth.InvalidRefreshToken", "The refresh token is invalid.");

        public static readonly Error RefreshTokenExpired = new(
            "Auth.RefreshTokenExpired", "The refresh token has expired.");

        public static readonly Error RefreshTokenRevoked = new(
            "Auth.RefreshTokenRevoked", "The refresh token has been revoked.");

        public static readonly Error RefreshTokenReused = new(
            "Auth.RefreshTokenReused", "Suspicious activity detected: Refresh token reuse. All sessions revoked.");
    }

    public static class User
    {
        public static readonly Error NotFound = new(
            "User.NotFound", "User was not found.");
        
        public static readonly Error AlreadyExists = new(
            "User.AlreadyExists", "User with this email already exists.");
        
        public static readonly Error UpdateFailed = new(
            "User.UpdateFailed", "Failed to update user.");
        
        public static readonly Error DeleteFailed = new(
            "User.DeleteFailed", "Failed to delete user.");
    }

    public static class General
    {
        public static readonly Error ServerError = new(
            "General.ServerError", "An unexpected error occurred.");

        public static readonly Error ValidationError = new(
            "General.ValidationError", "One or more validation errors occurred.");

        public static readonly Error NotFound = new(
            "General.NotFound", "The requested resource was not found.");

        public static readonly Error ArgumentError = new(
            "General.ArgumentError", "An invalid argument was provided.");

        public static readonly Error InvalidOperation = new(
            "General.InvalidOperation", "The requested operation is not valid in the current state.");

        public static readonly Error Forbidden = new(
            "General.Forbidden", "You are not authorized to access this resource.");
    }
}
