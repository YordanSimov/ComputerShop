using ComputerShop.Models.Configurations;
using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace ComputerShop.BL.Kafka
{
    public class KafkaProducerService<TKey, TValue> : IKafkaProducerService<TKey, TValue>
    {
        private readonly ProducerConfig config;
        private readonly IProducer<TKey, TValue> producer;
        private readonly string topicName;

        public IOptionsMonitor<KafkaProducerSettings> kafkaProducerSettings { get; set; }

        public KafkaProducerService(IOptionsMonitor<KafkaProducerSettings> kafkaProducerSettings,string topicName)
        {
            this.kafkaProducerSettings = kafkaProducerSettings;
            this.topicName = topicName;
            config = new ProducerConfig()
            {
                BootstrapServers = kafkaProducerSettings.CurrentValue.BootstrapServers
            };
            producer = new ProducerBuilder<TKey, TValue>(config)
                .SetKeySerializer(new KafkaSerializer<TKey>())
                .SetValueSerializer(new KafkaSerializer<TValue>()).Build();
        }
        public async Task Produce(TKey messageKey, TValue messageValue)
        {
            try
            {
                var msg = new Message<TKey, TValue>()
                {
                    Key = messageKey,
                    Value = messageValue
                };

                var result = await producer.ProduceAsync(topicName, msg);

                if (result != null)
                {
                    Console.WriteLine($"Delivered: {result.Value} to {result.TopicPartitionOffset}");
                }
            }
            catch (ProduceException<int, string> ex)
            {
                Console.WriteLine($"Delivery failed: {ex.Error.Reason}");
            }
        }
    }
}