namespace learnify.ai.api.Features.Authentication;

public record AuthResponse(
    string Token,
    string RefreshToken,
    DateTime ExpiresAt,
    UserInfo User
);

public record UserInfo(
    int Id,
    string Email,
    string FirstName,
    string LastName,
    string FullName,
    bool IsActive
);

public record RegisterResponse(
    int UserId,
    string Email,
    string Message
);

public record RefreshTokenResponse(
    string Token,
    string RefreshToken,
    DateTime ExpiresAt
);

public record ForgotPasswordResponse(
    bool Success,
    string Message
);

public record ResetPasswordResponse(
    bool Success,
    string Message
);

public record ChangePasswordResponse(
    bool Success,
    string Message
);

