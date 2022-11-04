namespace ComputerShop.Models.Configurations
{
    public class KafkaConsumerSettings
    {
        public string BootstrapServers { get; set; }

        public string GroupId { get; set; }

        public string PurchaseTopic { get; set; }

        public string InfoReportTopic { get; set; }
    }
}
