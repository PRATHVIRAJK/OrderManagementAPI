using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace API.Health
{
    public class DatabaseHealthCheck(IConfiguration configuration) : IHealthCheck
    {
        private readonly string _connectionString =
            configuration.GetConnectionString("DefaultConnection")!;

        public async Task<HealthCheckResult> CheckHealthAsync( HealthCheckContext context,CancellationToken cancellationToken = default)
        {
            try
            {
                await using var connection = new SqlConnection(_connectionString);

                await connection.OpenAsync(cancellationToken);

                using var command = new SqlCommand("SELECT 1", connection);

                await command.ExecuteScalarAsync(cancellationToken);

                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Database unavailable", ex);
            }
        }
    }
}
