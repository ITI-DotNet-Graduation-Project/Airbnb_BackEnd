namespace Airbnb.Application.DTOs.Authentication;

public record AuthResponse
(
    string id,
    string? Email,
    string FullName,
    string Taken,
    int ExpiresIn,
    string RefreshToken,
    DateTime RefreshTokenExpiration
);