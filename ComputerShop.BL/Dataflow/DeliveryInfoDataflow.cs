using ComputerShop.BL.Kafka;
using ComputerShop.DL.MongoRepositories;
using ComputerShop.Models.Configurations;
using ComputerShop.Models.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Threading.Tasks.Dataflow;

namespace ComputerShop.BL.Dataflow
{
    public class DeliveryInfoDataflow : IHostedService
    {
        private readonly IOptionsMonitor<KafkaConsumerSettings> kafkaConsumerSettings;
        private readonly IPurchaseRepository purchaseRepository;
        private readonly IUserRepository userRepository;
        private readonly KafkaConsumerService<Guid, DeliveryInfo> kafkaConsumerService;
        private TransformBlock<DeliveryInfo, DeliveryInfo> createValuesBlock;
        private ActionBlock<DeliveryInfo> updateBlock;

        public DeliveryInfoDataflow(IOptionsMonitor<KafkaConsumerSettings> kafkaConsumerSettings,
            IPurchaseRepository purchaseRepository,
            IUserRepository userRepository)
        {
            this.kafkaConsumerSettings = kafkaConsumerSettings;
            this.purchaseRepository = purchaseRepository;
            this.userRepository = userRepository;
            kafkaConsumerService = new KafkaConsumerService<Guid, DeliveryInfo>
                (kafkaConsumerSettings, HandleDeliveryInfo,kafkaConsumerSettings.CurrentValue.InfoReportTopic);

            createValuesBlock = new TransformBlock<DeliveryInfo, DeliveryInfo>(async deliveryInfo =>
            {
                var purchase = await purchaseRepository.GetPurchaseById(deliveryInfo.PurchaseId);
                var user =  await userRepository.GetUserById(purchase.UserId);

                deliveryInfo.Id = Guid.NewGuid();
                deliveryInfo.Address = user.Address;
                return deliveryInfo;
            });
            updateBlock = new ActionBlock<DeliveryInfo>(async result =>
            {
                var purchase = await purchaseRepository.GetPurchaseById(result.PurchaseId);
                purchase.DeliveryInfo = result;

                await purchaseRepository.UpdatePurchase(purchase);
            });
            createValuesBlock.LinkTo(updateBlock);
        }

        private void HandleDeliveryInfo(DeliveryInfo deliveryInfo)
        {
            createValuesBlock.Post(deliveryInfo);
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
