namespace ComputerShop.Models.Configurations
{
    public class KafkaProducerSettings
    {
        public string BootstrapServers { get; set; }

        public string PurchaseTopic { get; set; }

        public string InfoReportTopic { get; set; }
    }
}
