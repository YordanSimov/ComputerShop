using ComputerShop.DL.Interfaces;
using ComputerShop.Models.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace ComputerShop.DL.Repositories
{
    public class BrandRepository : IBrandRepository
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<BrandRepository> logger;

        public BrandRepository(IConfiguration configuration, ILogger<BrandRepository> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }
        public async Task<Brand> Add(Brand input)
        {
            try
            {
                await using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();

                    await connection.ExecuteAsync
                        (@"INSERT INTO Brands ([Name]) VALUES (@Name)",
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

        public async Task<Brand?> Delete(int id)
        {
            try
            {
                var brand = await GetById(id);
                await using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();

                    await connection.QueryFirstOrDefaultAsync<Brand>("DELETE FROM Brands WHERE Id=@Id", new { Id = id });
                    return brand;
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in {nameof(Delete)}: {ex.Message}", ex);
            }
            return null;
        }

        public async Task<IEnumerable<Brand>> GetAll()
        {
            try
            {
                await using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();

                    return await connection.QueryAsync<Brand>("SELECT * FROM Brands WITH (NOLOCK)");
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in {nameof(GetAll)}: {ex.Message}", ex);
            }
            return Enumerable.Empty<Brand>();
        }

        public async Task<Brand?> GetById(int id)
        {
            try
            {
                await using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();

                    return await connection.QueryFirstOrDefaultAsync<Brand>("SELECT * FROM Brands WITH (NOLOCK) WHERE Id = @Id", new { Id = id });
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in {nameof(GetById)}: {ex.Message}", ex);
            }
            return null;
        }

        public async Task<Brand?> GetByName(string name)
        {
            try
            {
                await using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();

                    return await connection.QueryFirstOrDefaultAsync<Brand>("SELECT * FROM Brands WITH (NOLOCK) WHERE [Name] = @Name", new { Name = name });
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in {nameof(GetByName)}: {ex.Message}", ex);
            }
            return null;
        }

        public async Task Update(Brand input)
        {
            try
            {
                await using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();

                    await connection.ExecuteAsync
                        ("UPDATE Brands SET [Name]=@Name WHERE Id = @Id",
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
