using ComputerShop.Models.Models;
using MediatR;

namespace ComputerShop.Models.MediatR.Commands.BrandCommands
{
    public record GetByNameBrandCommand(string Name) : IRequest<Brand?>
    {
    }
}
