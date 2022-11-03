using ComputerShop.Models.Responses;
using MediatR;

namespace ComputerShop.Models.MediatR.Commands.BrandCommands
{
    public record DeleteBrandCommand(int BrandId) : IRequest<BrandResponse>
    {
    }
}
