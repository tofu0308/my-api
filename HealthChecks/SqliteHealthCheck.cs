using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Data.Sqlite;

namespace MyApi.HealthChecks;

public class SqliteHealthCheck : IHealthCheck
{
    private readonly string _connectionString;

    public SqliteHealthCheck(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new ArgumentNullException(nameof(configuration), "Connection string 'DefaultConnection' not found.");
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);
            
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT 1";
            await command.ExecuteScalarAsync(cancellationToken);

            return HealthCheckResult.Healthy("SQLite database is accessible.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("SQLite database is not accessible.", ex);
        }
    }
} 