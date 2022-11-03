using ComputerShop.Models.Requests;
using ComputerShop.Models.Responses;
using MediatR;

namespace ComputerShop.Models.MediatR.Commands
{
    public record UpdateComputerCommand(ComputerRequest Computer) : IRequest<ComputerResponse>
    {
    }
}
