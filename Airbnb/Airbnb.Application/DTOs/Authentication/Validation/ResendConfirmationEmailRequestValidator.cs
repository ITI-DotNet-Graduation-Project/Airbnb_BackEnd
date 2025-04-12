using FluentValidation;
using Microsoft.AspNetCore.Identity.Data;

namespace Airbnb.Application.DTOs.Authentication;

public class ResendConfirmationEmailRequestValidator : AbstractValidator<ResendConfirmationEmailRequest>
{
    public ResendConfirmationEmailRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
