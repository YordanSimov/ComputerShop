using ComputerShop.Models.Models;
using MediatR;

namespace ComputerShop.Models.MediatR.Commands.UserCommands
{
    public record GetUserPurchasesCommand(int UserId) : IRequest<IEnumerable<Purchase>>
    {
    }
}
