using ComputerShop.Models.Requests;
using FluentValidation;

namespace ComputerShop.Validators
{
    public class PurchaseRequestValidator : AbstractValidator<PurchaseRequest>
    {
        public PurchaseRequestValidator()
        {
            RuleFor(x => x.ComputerId).GreaterThan(0).NotNull().NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}