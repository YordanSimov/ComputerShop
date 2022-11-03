using ComputerShop.Models.Responses;
using MediatR;

namespace ComputerShop.Models.MediatR.Commands
{
    public record DeleteComputerCommand(int ComputerId) : IRequest<ComputerResponse>
    {
    }
}
