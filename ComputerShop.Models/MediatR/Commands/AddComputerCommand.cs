using ComputerShop.Models.Requests;
using ComputerShop.Models.Responses;
using MediatR;

namespace ComputerShop.Models.MediatR.Commands
{
    public record AddComputerCommand(ComputerRequest Computer) : IRequest<ComputerResponse>
    {
    }
}
