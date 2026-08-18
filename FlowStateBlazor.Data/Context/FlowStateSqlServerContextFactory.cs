using FlowStateBlazor.Data.Data;
using FlowStateBlazor.Data.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FlowStateBlazor.Data.Context
{
    /// <summary>
    /// Factory de design-time pour FlowStateSqlServerContext
    /// Utilisée par dotnet ef migrations
    /// </summary>
    public class FlowStateSqlServerContextFactory : IDesignTimeDbContextFactory<FlowStateSqlServerContext>
    {
        public FlowStateSqlServerContext CreateDbContext(string[] args)
        {
            var configuration = DatabaseConnectionConfiguration.BuildConfiguration();
            var connectionConfig = new DatabaseConnectionConfiguration(configuration);
            var connectionString = connectionConfig.GetSqlServerConnectionString();

            var optionsBuilder = new DbContextOptionsBuilder<FlowStateSqlServerContext>();

            optionsBuilder.UseSqlServer(connectionString,
                options => options.EnableRetryOnFailure());

            return new FlowStateSqlServerContext(optionsBuilder.Options, false);
        }
    }
}
