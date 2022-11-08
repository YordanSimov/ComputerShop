using ComputerShop.Models.Requests;
using ComputerShop.Models.Responses;
using MediatR;

namespace ComputerShop.Models.MediatR.Commands.IdentityCommands
{
    public record CreateUserCommand(UserRequest UserRequest) : IRequest<UserResponse>
    {
    }
}
