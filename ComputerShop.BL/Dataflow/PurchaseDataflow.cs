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
        private readonly IUserRepository userRepository;
        private readonly IOptionsMonitor<KafkaConsumerSettings> kafkaConsumerSettings;
        private readonly KafkaConsumerService<Guid, Purchase> kafkaConsumerService;
        private readonly KafkaProducerService<Guid, DeliveryInfo> kafkaProducerService;
        private TransformBlock<Purchase, Purchase> addValuesBlock;
        private ActionBlock<Purchase> addToDBBlock;
        private ActionBlock<Purchase> produceDeliveryInfoBlock;
        private BroadcastBlock<Purchase> broadcastBlock;


        public PurchaseDataflow(IOptionsMonitor<KafkaConsumerSettings> kafkaConsumerSettings,
            IOptionsMonitor<KafkaProducerSettings> kafkaProducerSettings,
            IPurchaseRepository purchaseRepository,
            IComputerRepository computerRepository,
            IUserRepository userRepository)
        {
            this.purchaseRepository = purchaseRepository;
            this.computerRepository = computerRepository;
            this.userRepository = userRepository;

            this.kafkaConsumerSettings = kafkaConsumerSettings;
            kafkaConsumerService = new KafkaConsumerService<Guid, Purchase>
                (kafkaConsumerSettings, HandlePurchase, kafkaConsumerSettings.CurrentValue.PurchaseTopic);
            kafkaProducerService = new KafkaProducerService<Guid, DeliveryInfo>
                (kafkaProducerSettings, kafkaProducerSettings.CurrentValue.InfoReportTopic);

            broadcastBlock = new BroadcastBlock<Purchase>(x => x);

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
                    var userCheck = await userRepository.GetUserById(result.UserId);
                    if (userCheck != null)
                    {
                        await purchaseRepository.AddPurchase(result);
                    }                    
                }
            });
            produceDeliveryInfoBlock = new ActionBlock<Purchase>(async result =>
            {
                if (result != null)
                {
                    var deliveryInfo = new DeliveryInfo();
                    deliveryInfo.PurchaseId = result.Id;

                    await kafkaProducerService.Produce(Guid.NewGuid(), deliveryInfo);
                }
            });
            addValuesBlock.LinkTo(broadcastBlock);
            broadcastBlock.LinkTo(addToDBBlock);
            broadcastBlock.LinkTo(produceDeliveryInfoBlock);
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
