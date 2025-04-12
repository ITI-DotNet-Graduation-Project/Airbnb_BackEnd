namespace Airbnb.Application.DTOs.Authentication;

public record _RegisterRequest
(
    string Email,
    string Password,
    string FirstName,
    string LastName
);
