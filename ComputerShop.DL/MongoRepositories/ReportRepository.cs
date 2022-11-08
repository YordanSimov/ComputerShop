using ComputerShop.Models.Configurations;
using ComputerShop.Models.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ComputerShop.DL.MongoRepositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly ILogger<ReportRepository> logger;

        private readonly MongoClient dbClient;
        private readonly IMongoDatabase database;
        private readonly IMongoCollection<Report> collection;
        private readonly IOptionsMonitor<MongoDBSettings> mongoSettings;

        public ReportRepository(ILogger<ReportRepository> logger, IOptionsMonitor<MongoDBSettings> mongoSettings)
        {
            this.logger = logger;
            this.mongoSettings = mongoSettings;
            dbClient = new MongoClient(mongoSettings.CurrentValue.ConnectionString);
            database = dbClient.GetDatabase(mongoSettings.CurrentValue.DatabaseName);
            collection = database.GetCollection<Report>(mongoSettings.CurrentValue.CollectionReport);
        }

        public async Task<Report> CreateReportByTime(Report report)
        {
            try
            {
                await collection.InsertOneAsync(report);
                return report;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<IEnumerable<Report>> GetAllReports(DateTime? time)
        {
            try
            {
                if (time == null)
                {
                    var result = await collection.FindAsync(x => true);
                    return result.ToList();
                }
                else
                {
                    var result = await collection.FindAsync(x => x.ReportTime <= time);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return Enumerable.Empty<Report>();
            }
        }
    }
}
