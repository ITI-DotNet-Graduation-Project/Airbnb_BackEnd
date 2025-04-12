namespace Airbnb.Application.DTOs.Authentication;

public record _LoginRequest
(
    string email,
    string password
);
