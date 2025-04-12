using Airbnb.Application.Const;
using FluentValidation;

namespace Airbnb.Application.DTOs.Authentication;
public class ResetPasswordRequestValidator : AbstractValidator<_ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Code).NotEmpty();
        RuleFor(x => x.newPassword)
            .Matches(RegexPattern.StrongPassword)
            .WithMessage("Password must contains atleast 8 digits, one Uppercase,one Lowercase and NunAlphanumeric");


    }

}
