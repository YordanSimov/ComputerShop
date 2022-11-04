using ComputerShop.Models.Configurations;
using ComputerShop.Models.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ComputerShop.DL.MongoRepositories
{
    public class PurchaseRepository : IPurchaseRepository
    {
        private readonly ILogger<PurchaseRepository> logger;

        private readonly MongoClient dbClient;
        private readonly IMongoDatabase database;
        private readonly IMongoCollection<Purchase> collection;


        public PurchaseRepository(ILogger<PurchaseRepository> logger, IOptionsMonitor<MongoDBSettings> mongoSettings)
        {
            this.logger = logger;
            dbClient = new MongoClient(mongoSettings.CurrentValue.ConnectionString);
            database = dbClient.GetDatabase(mongoSettings.CurrentValue.DatabaseName);
            collection = database.GetCollection<Purchase>(mongoSettings.CurrentValue.CollectionPurchase);
        }
        public async Task<Purchase> AddPurchase(Purchase purchase)
        {
            try
            {
                await collection.InsertOneAsync(purchase);

                return purchase;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return null;
            }
        }
    }
}
