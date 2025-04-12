namespace Airbnb.Application.DTOs.Authentication;

public record ConfirmEmailRequest
(
    string UserId,
    string Code

);
