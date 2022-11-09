using ComputerShop.DL.Interfaces;
using ComputerShop.Models.Configurations;
using ComputerShop.Models.Models;
using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data.SqlClient;

namespace ComputerShop.DL.Repositories
{
    public class ComputerRepository : IComputerRepository
    {
        private readonly IOptionsMonitor<SQLServerSettings> sqlServerSettings;
        private readonly ILogger<ComputerRepository> logger;

        public ComputerRepository(IOptionsMonitor<SQLServerSettings> sqlServerSettings, ILogger<ComputerRepository> logger)
        {
            this.sqlServerSettings = sqlServerSettings;
            this.logger = logger;
        }
        public async Task<Computer> Add(Computer input)
        {
            try
            {
                await using (var connection = new SqlConnection(sqlServerSettings.CurrentValue.ConnectionString))
                {
                    await connection.OpenAsync();

                    await connection.ExecuteAsync
                        (@"INSERT INTO [Computers] (BrandId,[Name],Price,VideoCard,Processor,RAM) VALUES (@BrandId,@Name,@Price,@VideoCard,@Processor,@RAM)",
                        input);
                    return input;
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in {nameof(Add)}: {ex.Message}", ex);
            }
            return null;
        }

        public async Task<Computer?> Delete(int id)
        {
            try
            {
                var computer = await GetById(id);
                await using (var connection = new SqlConnection(sqlServerSettings.CurrentValue.ConnectionString))
                {
                    await connection.OpenAsync();

                    await connection.QueryFirstOrDefaultAsync<Computer>("DELETE FROM Computers WHERE Id=@Id", new { Id = id });
                    return computer;
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in {nameof(Delete)}: {ex.Message}", ex);
            }
            return null;
        }

        public async Task<IEnumerable<Computer>> GetAll()
        {
            try
            {
                await using (var connection = new SqlConnection(sqlServerSettings.CurrentValue.ConnectionString))
                {
                    await connection.OpenAsync();

                    return await connection.QueryAsync<Computer>("SELECT * FROM Computers WITH (NOLOCK)");
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in {nameof(GetAll)}: {ex.Message}", ex);
            }
            return Enumerable.Empty<Computer>();
        }

        public async Task<Computer?> GetById(int id)
        {
            try
            {
                await using (var connection = new SqlConnection(sqlServerSettings.CurrentValue.ConnectionString))
                {
                    await connection.OpenAsync();

                    return await connection.QueryFirstOrDefaultAsync<Computer>("SELECT * FROM Computers WITH (NOLOCK) WHERE Id = @Id", new { Id = id });
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in {nameof(GetById)}: {ex.Message}", ex);
            }
            return null;
        }

        public async Task<Computer?> GetByName(string name)
        {
            try
            {
                await using (var connection = new SqlConnection(sqlServerSettings.CurrentValue.ConnectionString))
                {
                    await connection.OpenAsync();

                    return await connection.QueryFirstOrDefaultAsync<Computer>("SELECT * FROM Computers WITH (NOLOCK) WHERE [Name] = @Name", new { Name = name });
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in {nameof(GetByName)}: {ex.Message}", ex);
            }
            return null;
        }

        public async Task Update(Computer input)
        {
            try
            {
                await using (var connection = new SqlConnection(sqlServerSettings.CurrentValue.ConnectionString))
                {
                    await connection.OpenAsync();

                    await connection.ExecuteAsync
                        ("UPDATE Computers SET BrandId = @BrandId,[Name]=@Name,VideoCard=@VideoCard,Processor=@Processor,Price=@Price,RAM=@RAM WHERE Id = @Id",
                        input);
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in {nameof(Update)}: {ex.Message}", ex);
            }
        }
    }
}