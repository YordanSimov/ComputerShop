using ComputerShop.Models.Models;
using MediatR;

namespace ComputerShop.Models.MediatR.Commands.BrandCommands
{
    public record GetAllBrandsCommand : IRequest<IEnumerable<Brand>>
    {
    }
}
