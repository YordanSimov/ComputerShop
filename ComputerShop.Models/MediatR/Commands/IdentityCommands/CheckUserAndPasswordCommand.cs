using ComputerShop.Models.Models;
using MediatR;

namespace ComputerShop.Models.MediatR.Commands.IdentityCommands
{
    public record CheckUserAndPasswordCommand(string UserName, string Password) : IRequest<User?>
    {
    }
}
