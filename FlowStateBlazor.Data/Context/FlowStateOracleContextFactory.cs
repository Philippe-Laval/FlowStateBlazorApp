using FlowStateBlazor.Data.Data;
using FlowStateBlazor.Data.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FlowStateBlazor.Data.Context
{
    /// <summary>
    /// Factory de design-time pour FlowStateOracleContext
    /// Utilisée par dotnet ef migrations
    /// </summary>
    public class FlowStateOracleContextFactory : IDesignTimeDbContextFactory<FlowStateOracleContext>
    {
        public FlowStateOracleContext CreateDbContext(string[] args)
        {
            var configuration = DatabaseConnectionConfiguration.BuildConfiguration();
            var connectionConfig = new DatabaseConnectionConfiguration(configuration);
            var connectionString = connectionConfig.GetOracleConnectionString();

            var optionsBuilder = new DbContextOptionsBuilder<FlowStateOracleContext>();

            optionsBuilder.UseOracle(connectionString);

            return new FlowStateOracleContext(optionsBuilder.Options);
        }
    }
}

