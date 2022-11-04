using ComputerShop.Models.Configurations;
using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace ComputerShop.BL.Kafka
{
    public class KafkaConsumerService<TKey, TValue> : IKafkaConsumerService<TKey, TValue>
    {
        private readonly ConsumerConfig config;
        private readonly IConsumer<TKey, TValue> consumer;
        private Action<TValue> action;
        private readonly string topicName;

        public IOptionsMonitor<KafkaConsumerSettings> kafkaConsumerSettings { get; set; }

        public KafkaConsumerService(IOptionsMonitor<KafkaConsumerSettings> kafkaConsumerSettings,Action<TValue> action,string topicName)
        {
            this.kafkaConsumerSettings = kafkaConsumerSettings;
            this.action = action;
            this.topicName = topicName;
            config = new ConsumerConfig()
            {
                BootstrapServers = kafkaConsumerSettings.CurrentValue.BootstrapServers,
                GroupId = kafkaConsumerSettings.CurrentValue.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            consumer = new ConsumerBuilder<TKey, TValue>(config)
                .SetKeyDeserializer(new KafkaDeserializer<TKey>())
                .SetValueDeserializer(new KafkaDeserializer<TValue>()).Build();
        }
        public void Consume(CancellationToken cancellationToken)
        {
            consumer.Subscribe(topicName);
            while (!cancellationToken.IsCancellationRequested)
            {
                var cr = consumer.Consume(cancellationToken);
                action.Invoke(cr.Message.Value);
            }
        }
    }
}
