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
            "Auth.InvalidCredentials", "Invalid phone number or OTP code.");
        
        public static readonly Error InvalidPhoneNumber = new(
            "Auth.InvalidPhoneNumber", "Phone number is invalid or too short.");
        
        public static readonly Error InvalidOtp = new(
            "Auth.InvalidOtp", "OTP code is invalid, expired, or already used.");
        
        public static readonly Error UserNotFound = new(
            "Auth.UserNotFound", "User was not found.");
        
        public static readonly Error TokenNotFound = new(
            "Auth.TokenNotFound", "Authentication token was not found.");
        
        public static readonly Error TokenExpired = new(
            "Auth.TokenExpired", "Authentication token has expired.");
        
        public static readonly Error Unauthorized = new(
            "Auth.Unauthorized", "You are not authorized to perform this action.");
    }

    public static class User
    {
        public static readonly Error NotFound = new(
            "User.NotFound", "User was not found.");
        
        public static readonly Error AlreadyExists = new(
            "User.AlreadyExists", "User with this phone number already exists.");
        
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
    }
}
