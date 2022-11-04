using ComputerShop.Models.Requests;
using ComputerShop.Models.Responses;
using MediatR;

namespace ComputerShop.Models.MediatR.Commands.PurchaseCommands
{
    public record AddPurchaseCommand(PurchaseRequest Purchase) : IRequest<PurchaseResponse>
    {
    }
}