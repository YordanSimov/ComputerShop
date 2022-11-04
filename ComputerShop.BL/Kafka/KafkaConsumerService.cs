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

        public IOptionsMonitor<KafkaConsumerSettings> kafkaConsumerSettings { get; set; }

        public KafkaConsumerService(IOptionsMonitor<KafkaConsumerSettings> kafkaConsumerSettings,Action<TValue> action)
        {
            this.kafkaConsumerSettings = kafkaConsumerSettings;
            this.action = action;
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
            consumer.Subscribe($"{typeof(TValue).Name}ProjectTopic");
            while (!cancellationToken.IsCancellationRequested)
            {
                var cr = consumer.Consume(cancellationToken);
                action.Invoke(cr.Message.Value);
            }
        }
    }
}
