using ComputerShop.Models.Models;
using MediatR;

namespace ComputerShop.Models.MediatR.Commands
{
    public record GetByNameComputerCommand(string Name) : IRequest<Computer?>
    {
    }
}
