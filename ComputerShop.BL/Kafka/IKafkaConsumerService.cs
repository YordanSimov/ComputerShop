namespace ComputerShop.BL.Kafka
{
    public interface IKafkaConsumerService<TKey, TValue>
    {
        void Consume(CancellationToken cancellationToken);
    }
}
