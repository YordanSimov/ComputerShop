using ComputerShop.DL.MongoRepositories;
using ComputerShop.Models.MediatR.Commands.UserCommands;
using ComputerShop.Models.Models;
using MediatR;

namespace ComputerShop.BL.CommandHandlers.UserCommandHandlers
{
    public class GetUserPurchasesCommandHandler : IRequestHandler<GetUserPurchasesCommand, IEnumerable<Purchase>>
    {
        private readonly IPurchaseRepository purchaseRepository;

        public GetUserPurchasesCommandHandler(IPurchaseRepository purchaseRepository)
        {
            this.purchaseRepository = purchaseRepository;
        }

        public async Task<IEnumerable<Purchase>> Handle(GetUserPurchasesCommand request, CancellationToken cancellationToken)
        {
            return await purchaseRepository.GetUserPurchases(request.UserId);
        }
    }
}
