using AutoMapper;
using ComputerShop.BL.Kafka;
using ComputerShop.DL.Interfaces;
using ComputerShop.Models.MediatR.Commands.PurchaseCommands;
using ComputerShop.Models.Models;
using ComputerShop.Models.Responses;
using MediatR;
using System.Net;

namespace ComputerShop.BL.CommandHandlers.PurchaseCommandHandlers
{
    public class AddPurchaseCommandHandler : IRequestHandler<AddPurchaseCommand,PurchaseResponse>
    {
        private readonly IMapper mapper;
        private readonly IComputerRepository computerRepository;
        private readonly IKafkaProducerService<Guid, Purchase> kafkaProducerService;

        public AddPurchaseCommandHandler(IMapper mapper,
            IComputerRepository computerRepository,
            IKafkaProducerService<Guid,Purchase> kafkaProducerService)
        {
            this.mapper = mapper;
            this.computerRepository = computerRepository;
            this.kafkaProducerService = kafkaProducerService;
        }

        public async Task<PurchaseResponse> Handle(AddPurchaseCommand request, CancellationToken cancellationToken)
        {
            var purchase = mapper.Map<Purchase>(request.Purchase);
            var purchaseValues = await computerRepository.GetById(purchase.ComputerId);

            purchase.Id = Guid.NewGuid();
            purchase.ComputerName = purchaseValues.Name;
            purchase.ComputerPrice = purchaseValues.Price;

            await kafkaProducerService.Produce(purchase.Id,purchase);

            return new PurchaseResponse()
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Successfully purchased computer",
                Purchase = purchase
            };
        }
    }
}
