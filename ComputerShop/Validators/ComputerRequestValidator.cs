using ComputerShop.Models.Requests;
using FluentValidation;

namespace ComputerShop.Validators
{
    public class ComputerRequestValidator : AbstractValidator<ComputerRequest>
    {
        public ComputerRequestValidator()
        {
            RuleFor(x => x.BrandId).NotNull().GreaterThan(0).WithMessage("BrandId should be greater than 0");
            RuleFor(x => x.Price).NotNull().NotEmpty();
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price should be greater than 0");
            RuleFor(x => x.Name).NotNull().NotEmpty();
            RuleFor(x => x.RAM).GreaterThan(0);
            When(x => !string.IsNullOrEmpty(x.Name), () =>
            {
                RuleFor(x => x.Name).MinimumLength(3).MaximumLength(80).WithMessage("Name not in range");
            });
        }
    }
}
