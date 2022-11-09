using ComputerShop.Models.Models;
using MediatR;

namespace ComputerShop.Models.MediatR.Commands.UserCommands
{
    public record GetAllUsersCommand : IRequest<IEnumerable<User>>
    {
    }
}