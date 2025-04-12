using FluentValidation;

namespace Airbnb.Application.DTOs.Authentication;

public class ForgetPasswordRequestValidator : AbstractValidator<ForgetPasswordRequest>
{
    public ForgetPasswordRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
