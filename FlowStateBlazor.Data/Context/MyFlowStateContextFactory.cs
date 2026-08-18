using FlowStateBlazor.Data.Data;
using FlowStateBlazor.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace FlowStateBlazor.Data.Context
{
    public class MyFlowStateContextFactory : IDbContextFactory<FlowStateContext>
    {
        IConfiguration _configuration;
        ILoggerFactory _loggerFactory;
        DatabaseSettings? _databaseSettings = null;

        public bool IsAzureDB { get; set; } = false;

        public MyFlowStateContextFactory(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _loggerFactory = loggerFactory;

            IConfigurationSection databaseSection = _configuration.GetSection("Database");
            _databaseSettings = databaseSection.Get<DatabaseSettings>();
        }

        public FlowStateContext CreateDbContext()
        {
            FlowStateContext context;

            string? connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new Exception("DefaultConnection is not set in the configuration file");
            }

            if (_databaseSettings is not null && 
                string.Compare(_databaseSettings.DatabaseType, "SQLSERVER", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
#if DEBUG
                // https://learn.microsoft.com/en-us/ef/core/logging-events-diagnostics/simple-logging
                var optionsBuilder = new DbContextOptionsBuilder<FlowStateSqlServerContext>()
                    .UseSqlServer(connectionString,
                                  options => options.EnableRetryOnFailure())
                    .UseLoggerFactory(_loggerFactory)
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors();
#else
                var optionsBuilder = new DbContextOptionsBuilder<FlowStateSqlServerContext>()
                    .UseSqlServer(connectionString,
                                  options => options.EnableRetryOnFailure());
#endif
                var options = optionsBuilder.Options;
                context = new FlowStateSqlServerContext(options, IsAzureDB);
            }
            else if (_databaseSettings is not null && 
                string.Compare(_databaseSettings.DatabaseType, "ORACLE", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
#if DEBUG
                // https://learn.microsoft.com/en-us/ef/core/logging-events-diagnostics/simple-logging
                var optionsBuilder = new DbContextOptionsBuilder<FlowStateOracleContext>()
                    .UseOracle(connectionString)
                    .UseLoggerFactory(_loggerFactory)
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors();
#else
                var optionsBuilder = new DbContextOptionsBuilder<FlowStateOracleContext>()
                    .UseOracle(connectionString);
#endif
                var options = optionsBuilder.Options;
                context = new FlowStateOracleContext(options);
            }
            else if (_databaseSettings is not null &&
                string.Compare(_databaseSettings.DatabaseType, "SQLITE", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
#if DEBUG
                // https://learn.microsoft.com/en-us/ef/core/logging-events-diagnostics/simple-logging
                var optionsBuilder = new DbContextOptionsBuilder<FlowStateSqliteContext>()
                    .UseSqlite(connectionString)
                    .UseLoggerFactory(_loggerFactory)
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors();
#else
                var optionsBuilder = new DbContextOptionsBuilder<FlowStateSqliteContext>()
                    .UseSqlite(connectionString);
#endif
                var options = optionsBuilder.Options;
                context = new FlowStateSqliteContext(options);
            }
            else
            {
                throw new Exception("DatabaseType can be either SQLSERVER, ORACLE or SQLITE");
            }

            // bool doMigration = false;
            // try
            // {
            //     doMigration = !context.Database.CompatibleWithModel(true);
            // }
            // catch (NotSupportedException)
            // {
            //     //if there are no metadata for migration
            //     doMigration = true;
            // }
            //
            // if (doMigration)
            // {
            //     var migrationConfig = new DbMigrationsConfiguration<MyContext>();
            //     migrationConfig.AutomaticMigrationDataLossAllowed = false;
            //     migrationConfig.AutomaticMigrationsEnabled = true;
            //     migrationConfig.TargetDatabase = connectionInfo;
            //     var migrator = new DbMigrator(migrationConfig);
            //     migrator.Update();
            // }

            return context;
        }

    }
}
