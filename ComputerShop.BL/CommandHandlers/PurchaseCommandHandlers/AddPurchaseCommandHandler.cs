using AutoMapper;
using ComputerShop.BL.Kafka;
using ComputerShop.Models.Configurations;
using ComputerShop.Models.MediatR.Commands.PurchaseCommands;
using ComputerShop.Models.Models;
using ComputerShop.Models.Responses;
using MediatR;
using Microsoft.Extensions.Options;
using System.Net;

namespace ComputerShop.BL.CommandHandlers.PurchaseCommandHandlers
{
    public class AddPurchaseCommandHandler : IRequestHandler<AddPurchaseCommand, PurchaseResponse>
    {
        private readonly IMapper mapper;
        private readonly IOptionsMonitor<KafkaProducerSettings> kafkaProducerSettings;
        private readonly KafkaProducerService<Guid, Purchase> kafkaProducerService;

        public AddPurchaseCommandHandler(IMapper mapper,IOptionsMonitor<KafkaProducerSettings> kafkaProducerSettings)
        {
            this.mapper = mapper;
            this.kafkaProducerSettings = kafkaProducerSettings;
            this.kafkaProducerService = 
                new KafkaProducerService<Guid, Purchase>(kafkaProducerSettings,kafkaProducerSettings.CurrentValue.PurchaseTopic);
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
