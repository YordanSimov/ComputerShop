using MediatR;

namespace ComputerShop.Models.MediatR.Commands.IdentityCommands
{
    public record GetUserRoleCommand(string UserName) : IRequest<string>
    {
    }
}
