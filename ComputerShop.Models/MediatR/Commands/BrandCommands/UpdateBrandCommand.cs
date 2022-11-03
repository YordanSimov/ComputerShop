using ComputerShop.Models.Requests;
using ComputerShop.Models.Responses;
using MediatR;

namespace ComputerShop.Models.MediatR.Commands.BrandCommands
{
    public record UpdateBrandCommand(BrandRequest Brand) : IRequest<BrandResponse>
    {
    }
}
