using ComputerShop.Models.Models;
using MediatR;

namespace ComputerShop.Models.MediatR.Commands
{
    public record GetAllComputersCommand : IRequest<IEnumerable<Computer>>
    {
    }
}
