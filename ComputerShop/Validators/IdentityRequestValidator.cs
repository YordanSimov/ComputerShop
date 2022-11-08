using ComputerShop.Models.Requests;
using FluentValidation;

namespace ComputerShop.Validators
{
    public class IdentityRequestValidator : AbstractValidator<LoginRequest>
    {
        public IdentityRequestValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().NotNull().MinimumLength(3).MaximumLength(15);
            RuleFor(x => x.Password).NotEmpty().NotNull().MinimumLength(3).MaximumLength(25);
        }
    }
}