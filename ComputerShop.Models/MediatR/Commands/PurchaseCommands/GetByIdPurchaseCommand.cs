using ComputerShop.Models.Models;
using MediatR;

namespace ComputerShop.Models.MediatR.Commands.PurchaseCommands
{
    public record GetByIdPurchaseCommand(Guid PurchaseId) : IRequest<Purchase?>
    {
    }
}
