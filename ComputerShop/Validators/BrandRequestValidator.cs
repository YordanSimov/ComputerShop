using ComputerShop.Models.Requests;
using FluentValidation;

namespace ComputerShop.Validators
{
    public class BrandRequestValidator : AbstractValidator<BrandRequest>
    {
        public BrandRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().NotNull();
            When(x => !string.IsNullOrEmpty(x.Name) && !x.Name.Any(char.IsDigit), () =>
            {
                RuleFor(x => x.Name).MinimumLength(2).MaximumLength(12).WithMessage("Name not in range");
            });
        }
    }
}
