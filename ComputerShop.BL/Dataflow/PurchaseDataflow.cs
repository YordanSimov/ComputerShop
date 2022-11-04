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
        private readonly IPurchaseRepository purchaseRepository;
        private readonly IComputerRepository computerRepository;
        private readonly IOptionsMonitor<KafkaConsumerSettings> kafkaConsumerSettings;
        private readonly KafkaConsumerService<Guid, Purchase> kafkaConsumerService;
        private readonly KafkaProducerService<Guid, DeliveryInfo> kafkaProducerService;
        private TransformBlock<Purchase, Purchase> addValuesBlock;
        private ActionBlock<Purchase> addToDBBlock;


        public PurchaseDataflow(IOptionsMonitor<KafkaConsumerSettings> kafkaConsumerSettings,
            IOptionsMonitor<KafkaProducerSettings> kafkaProducerSettings,
            IPurchaseRepository purchaseRepository,
            IComputerRepository computerRepository)
        {
            this.purchaseRepository = purchaseRepository;
            this.computerRepository = computerRepository;

            this.kafkaConsumerSettings = kafkaConsumerSettings;
            this.kafkaConsumerService = new KafkaConsumerService<Guid, Purchase>
                (kafkaConsumerSettings,HandlePurchase,kafkaConsumerSettings.CurrentValue.PurchaseTopic);
            this.kafkaProducerService = new KafkaProducerService<Guid, DeliveryInfo>
                (kafkaProducerSettings,kafkaProducerSettings.CurrentValue.InfoReportTopic);

            addValuesBlock = new TransformBlock<Purchase, Purchase>(async purchase =>
            {
                var computer = await computerRepository.GetById(purchase.ComputerId);

                purchase.ComputerPrice = computer.Price;
                purchase.ComputerName = computer.Name;
                purchase.TimeCreated = DateTime.Now;

                return purchase;
            });
            addToDBBlock = new ActionBlock<Purchase>(async result =>
            {
                if (result != null)
                {
                    await purchaseRepository.AddPurchase(result);

                    var deliveryInfo = new DeliveryInfo();
                    deliveryInfo.PurchaseId = result.Id;

                    await kafkaProducerService.Produce(Guid.NewGuid(),deliveryInfo);
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
