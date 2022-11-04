namespace ComputerShop.Models.Configurations
{
    public class MongoDBSettings
    {
        public string ConnectionString { get; set; }

        public string DatabaseName { get; set; }

        public string CollectionPurchase { get; set; }

        public string CollectionReport { get; set; }
    }
}