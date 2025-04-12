namespace Airbnb.Application.DTOs.Authentication;

public record ChangePasswordRequest
(
    string currentPassword,
    string newPassword
);
