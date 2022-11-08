using ComputerShop.Models.Configurations;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ComputerShop.HealthChecks
{
    public class MongoDBHealthcheck : IHealthCheck
    {
        private readonly IOptionsMonitor<MongoDBSettings> mongoSettings;
        private readonly IMongoClient dbClient;
        private readonly IMongoDatabase database;

        public MongoDBHealthcheck(IOptionsMonitor<MongoDBSettings> mongoSettings)
        {
            this.mongoSettings = mongoSettings;
            dbClient = new MongoClient(mongoSettings.CurrentValue.ConnectionString);
            database = dbClient.GetDatabase(mongoSettings.CurrentValue.DatabaseName);
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                await database.RunCommandAsync((Command<BsonDocument>)"{ping:1}");
            }

            catch (MongoException ex)
            {
                return HealthCheckResult.Unhealthy(ex.Message);
            }

            return HealthCheckResult.Healthy("MongoDB connection is OK");
        }
    }
}
