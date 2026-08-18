using FlowStateBlazor.Data.Data;
using FlowStateBlazor.Data.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FlowStateBlazor.Data.Context
{
    /// <summary>
    /// Factory de design-time pour FlowStateSqliteContext
    /// Utilisée par dotnet ef migrations
    /// </summary>
    public class FlowStateSqliteContextFactory : IDesignTimeDbContextFactory<FlowStateSqliteContext>
    {
        public FlowStateSqliteContext CreateDbContext(string[] args)
        {
            var configuration = DatabaseConnectionConfiguration.BuildConfiguration();
            var connectionConfig = new DatabaseConnectionConfiguration(configuration);
            var connectionString = connectionConfig.GetSqliteConnectionString();

            var optionsBuilder = new DbContextOptionsBuilder<FlowStateSqliteContext>();

            optionsBuilder.UseSqlite(connectionString);

            return new FlowStateSqliteContext(optionsBuilder.Options);
        }
    }
}
