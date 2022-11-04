using ComputerShop.BL.Kafka;
using ComputerShop.DL.Interfaces;
using ComputerShop.DL.MongoRepositories;
using ComputerShop.Models.Configurations;
using ComputerShop.Models.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Threading.Tasks.Dataflow;

namespace ComputerShop.BL.Dataflow
{
    public class PurchaseDataflow : IHostedService
    {
        private readonly IOptionsMonitor<KafkaConsumerSettings> kafkaConsumerSettings;
        private readonly IPurchaseRepository purchaseRepository;
        private readonly IComputerRepository computerRepository;
        private readonly KafkaConsumerService<Guid, Purchase> kafkaConsumerService;
        private TransformBlock<Purchase, Purchase> addValuesBlock;
        private ActionBlock<Purchase> addToDBBlock;


        public PurchaseDataflow(IOptionsMonitor<KafkaConsumerSettings> kafkaConsumerSettings,
            IPurchaseRepository purchaseRepository,
            IComputerRepository computerRepository)
        {
            this.kafkaConsumerSettings = kafkaConsumerSettings;
            this.purchaseRepository = purchaseRepository;
            this.computerRepository = computerRepository;
            this.kafkaConsumerService = new KafkaConsumerService<Guid, Purchase>(kafkaConsumerSettings,HandlePurchase);

            addValuesBlock = new TransformBlock<Purchase, Purchase>(async purchase =>
            {
                var computer = await computerRepository.GetById(purchase.ComputerId);

                purchase.ComputerPrice = computer.Price;
                purchase.ComputerName = computer.Name;

                return purchase;
            });
            addToDBBlock = new ActionBlock<Purchase>(async result =>
            {
                if (result != null)
                {
                    await purchaseRepository.AddPurchase(result);
                }
            });
            addValuesBlock.LinkTo(addToDBBlock);
        }

        private void HandlePurchase(Purchase purchase)
        {
            addValuesBlock.Post(purchase);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(() => kafkaConsumerService.Consume(cancellationToken));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
