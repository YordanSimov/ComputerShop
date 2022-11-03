using ComputerShop.Models.Models;
using MediatR;

namespace ComputerShop.Models.MediatR.Commands
{
    public record GetByIdComputerCommand(int ComputerId) : IRequest<Computer?>
    {
    }
}
