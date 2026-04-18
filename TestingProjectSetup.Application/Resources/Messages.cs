namespace TestingProjectSetup.Application.Resources;

/// <summary>
/// Centralized application messages for validation and domain errors
/// </summary>
public static class Messages
{
    public static class Validation
    {
        public const string FieldRequired = "A required field is missing.";
        public const string InvalidType = "Field has an invalid data type.";
        public const string ValidationError = "An error occurred during validation.";

        // Auth
        public const string EmailRequired = "Email is required.";
        public const string ValidEmailRequired = "A valid email address is required.";
        public const string PasswordRequired = "Password is required.";
        public const string PasswordMinLength = "Password must be at least 6 characters.";
        public const string PasswordMinLength8 = "New password must be at least 8 characters.";
        public const string NewPasswordDifferent = "New password must be different from the current password.";
        public const string CurrentPasswordRequired = "Current password is required.";
        public const string FcmTokenNotEmpty = "FCM token cannot be empty if provided.";
        public const string InvalidDeviceType = "Invalid device type specified.";
        public const string LanguageNotEmpty = "Language cannot be empty if provided.";
        public const string LanguageMaxLength = "Language code must not exceed 10 characters.";
        public const string InvalidRole = "Invalid role specified.";

        // OTP
        public const string OtpRequired = "OTP is required.";
        public const string OtpMustBe6Chars = "OTP must be exactly 6 characters.";
        public const string OtpMustBeNumeric = "OTP must be numeric.";

        // User
        public const string NameRequired = "Name is required.";
        public const string NameMaxLength = "Name must not exceed 100 characters.";
    }

    public static class Errors
    {
        // Auth Errors
        public const string InvalidCredentials = "Invalid email or password.";
        public const string UserNotFound = "User was not found.";
        public const string AccountInactive = "Your account is inactive. Please contact HR.";
        public const string CompanyInactive = "Your company's account is currently inactive. Please contact support.";
        public const string TokenNotFound = "Authentication token was not found.";
        public const string TokenExpired = "Authentication token has expired.";
        public const string Unauthorized = "You are not authorized to perform this action.";
        public const string InvalidRefreshToken = "The refresh token is invalid.";
        public const string RefreshTokenExpired = "The refresh token has expired.";
        public const string RefreshTokenRevoked = "The refresh token has been revoked.";
        public const string RefreshTokenReused = "Suspicious activity detected: Refresh token reuse. All sessions revoked.";
        public const string ResetFailed = "Failed to reset password.";
        public const string PasswordChangeFailed = "Failed to change password.";

        // User Errors
        public const string UserNotFoundError = "User was not found.";
        public const string UserAlreadyExists = "User with this email already exists.";
        public const string UserUpdateFailed = "Failed to update user.";
        public const string UserDeleteFailed = "Failed to delete user.";
        public const string InvalidOtp = "Invalid OTP code provided.";
        public const string OtpMaxAttemptsReached = "Maximum OTP attempts reached. Please request a new code.";

        // General
        public const string ServerError = "An unexpected error occurred.";
        public const string ValidationErrorGeneral = "One or more validation errors occurred.";
        public const string ResourceNotFound = "The requested resource was not found.";
        public const string ArgumentError = "An invalid argument was provided.";
        public const string InvalidOperationError = "The requested operation is not valid in the current state.";
        public const string ForbiddenError = "You are not authorized to access this resource.";
    }
}
