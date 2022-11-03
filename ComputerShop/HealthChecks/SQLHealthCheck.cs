using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Data.SqlClient;

namespace ComputerShop.HealthChecks
{
    public class SQLHealthCheck : IHealthCheck
    {
        private readonly IConfiguration configuration;
        public SQLHealthCheck(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                try
                {
                    await connection.OpenAsync();
                }
                catch (SqlException ex)
                {
                    return HealthCheckResult.Unhealthy(ex.Message);
                }
            }
            return HealthCheckResult.Healthy("SQL connection is OK");
        }
    }
}
