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
        private readonly IOptionsMonitor<MongoDBSettings> mongoSettings;


        public PurchaseRepository(ILogger<PurchaseRepository> logger, IOptionsMonitor<MongoDBSettings> mongoSettings)
        {
            this.logger = logger;
            this.mongoSettings = mongoSettings;
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

        public async Task<Purchase> GetPurchaseById(Guid id)
        {
            try
            {
                var result = await collection.FindAsync(x => x.Id == id);
                return await result.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<IEnumerable<Purchase>> GetPurchasesByTime(DateTime time)
        {
            try
            {
                var purchases = await collection.FindAsync(x => x.TimeCreated < time);
                return purchases.ToEnumerable();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<IEnumerable<Purchase>> GetUserPurchases(Guid userId)
        {
            try
            {
                var purchases = await collection.FindAsync(x => x.UserId == userId);
                return purchases.ToList();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return Enumerable.Empty<Purchase>();
            }
        }

        public async Task UpdatePurchase(Purchase purchase)
        {
            try
            {
               await collection.UpdateOneAsync(x => x.Id == purchase.Id, 
                    Builders<Purchase>.Update.Set(x => x.DeliveryInfo, purchase.DeliveryInfo));
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }
    }
}
