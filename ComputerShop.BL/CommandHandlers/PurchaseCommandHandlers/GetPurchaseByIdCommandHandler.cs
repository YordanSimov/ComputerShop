using ComputerShop.DL.MongoRepositories;
using ComputerShop.Models.MediatR.Commands.PurchaseCommands;
using ComputerShop.Models.Models;
using MediatR;

namespace ComputerShop.BL.CommandHandlers.PurchaseCommandHandlers
{
    public class GetPurchaseByIdCommandHandler : IRequestHandler<GetByIdPurchaseCommand, Purchase?>
    {
        private readonly IPurchaseRepository purchaseRepository;

        public GetPurchaseByIdCommandHandler(IPurchaseRepository purchaseRepository)
        {
            this.purchaseRepository = purchaseRepository;
        }
        public async Task<Purchase?> Handle(GetByIdPurchaseCommand request, CancellationToken cancellationToken)
        {
            return await purchaseRepository.GetPurchaseById(request.PurchaseId);
        }
    }
}
