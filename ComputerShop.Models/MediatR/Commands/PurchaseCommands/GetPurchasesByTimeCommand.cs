using ComputerShop.Models.Models;
using MediatR;

namespace ComputerShop.Models.MediatR.Commands.PurchaseCommands
{
    public record GetPurchasesByTimeCommand(DateTime Time) : IRequest<IEnumerable<Purchase>>
    {
    }
}