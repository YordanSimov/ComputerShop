using ComputerShop.DL.MongoRepositories;
using ComputerShop.Models.MediatR.Commands.PurchaseCommands;
using ComputerShop.Models.Models;
using MediatR;

namespace ComputerShop.BL.CommandHandlers.PurchaseCommandHandlers
{
    public class GetPurchasesByTimeCommandHandler : IRequestHandler<GetPurchasesByTimeCommand, IEnumerable<Purchase>>
    {
        private readonly IPurchaseRepository purchaseRepository;

        public GetPurchasesByTimeCommandHandler(IPurchaseRepository purchaseRepository)
        {
            this.purchaseRepository = purchaseRepository;
        }
        public async Task<IEnumerable<Purchase>> Handle(GetPurchasesByTimeCommand request, CancellationToken cancellationToken)
        {
            return await purchaseRepository.GetPurchasesByTime(request.Time);
        }
    }
}
