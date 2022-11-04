using AutoMapper;
using ComputerShop.BL.Kafka;
using ComputerShop.Models.MediatR.Commands.PurchaseCommands;
using ComputerShop.Models.Models;
using ComputerShop.Models.Responses;
using MediatR;
using System.Net;

namespace ComputerShop.BL.CommandHandlers.PurchaseCommandHandlers
{
    public class AddPurchaseCommandHandler : IRequestHandler<AddPurchaseCommand, PurchaseResponse>
    {
        private readonly IMapper mapper;
        private readonly IKafkaProducerService<Guid, Purchase> kafkaProducerService;

        public AddPurchaseCommandHandler(IMapper mapper,
            IKafkaProducerService<Guid, Purchase> kafkaProducerService)
        {
            this.mapper = mapper;
            this.kafkaProducerService = kafkaProducerService;
        }

        public async Task<PurchaseResponse> Handle(AddPurchaseCommand request, CancellationToken cancellationToken)
        {
            var purchase = mapper.Map<Purchase>(request.Purchase);

            purchase.Id = Guid.NewGuid();

            await kafkaProducerService.Produce(purchase.Id, purchase);

            return new PurchaseResponse()
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Successfully purchased computer",
                Purchase = purchase
            };
        }
    }
}
