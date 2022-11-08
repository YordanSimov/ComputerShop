using ComputerShop.Models.Configurations;
using ComputerShop.Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ComputerShop.DL.MongoRepositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ILogger<UserRepository> logger;

        private readonly MongoClient dbClient;
        private readonly IMongoDatabase database;
        private readonly IMongoCollection<User> collection;
        private readonly IOptionsMonitor<MongoDBSettings> mongoSettings;

        public UserRepository(ILogger<UserRepository> logger, IOptionsMonitor<MongoDBSettings> mongoSettings)
        {
            this.logger = logger;
            this.mongoSettings = mongoSettings;
            dbClient = new MongoClient(mongoSettings.CurrentValue.ConnectionString);
            database = dbClient.GetDatabase(mongoSettings.CurrentValue.DatabaseName);
            collection = database.GetCollection<User>(mongoSettings.CurrentValue.CollectionUser);
        }
        public async Task<IdentityResult> CreateUser(User user, CancellationToken cancellationToken)
        {
            try
            {
                var users = await GetAllUsers();
                if (users.Count() <= 0)
                {
                    user.Role = mongoSettings.CurrentValue.AdminRole;
                }
                else
                {
                    user.Role = mongoSettings.CurrentValue.DefaultRole;
                }
                await collection.InsertOneAsync(user);
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return IdentityResult.Failed(new IdentityError() { Description = "Error creating user" });
            }
        }

        public async Task<User?> GetUserByUserName(string username)
        {
            try
            {
                var usernameCheck = await collection.FindAsync(x => x.UserName == username);
                var user = await usernameCheck.FirstOrDefaultAsync();
                if (user != null)
                {
                    return user;
                }
                return null;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);

                return null;
            }
        }

        public async Task<User?> GetUserById(Guid userId)
        {
            try
            {
                var userCheck = await collection.FindAsync(x => x.Id == userId);
                var userValue = await userCheck.FirstOrDefaultAsync();
                if (userValue != null)
                {
                    return userValue;
                }
                return null;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return null;
            }

        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            try
            {
                var result = await collection.FindAsync(x => true);
                return result.ToList();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return Enumerable.Empty<User>();
            }
        }
    }
}
