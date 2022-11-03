using Confluent.Kafka;
using MessagePack;

namespace ComputerShop.BL.Kafka
{
    public class KafkaSerializer<T> : ISerializer<T>
    {
        public byte[] Serialize(T data, SerializationContext context)
        {
            return MessagePackSerializer.Serialize(data);
        }
    }
}
