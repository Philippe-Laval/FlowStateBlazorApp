using FlowStateBlazor.Data.EntityTypeConfigurations.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FlowStateBlazor.Data.Data
{

    public partial class FlowStateSqlServerContext : FlowStateContext
    {
        private bool IsAzureDB = false;

        public FlowStateSqlServerContext(DbContextOptions<FlowStateSqlServerContext> options, bool isAzureDB = false)
            : base(options)
        {
            IsAzureDB = isAzureDB;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=.\SQLExpress;Database=RSE;Integrated Security=True", options => options.EnableRetryOnFailure());
            }

#if DEBUG
            optionsBuilder.EnableSensitiveDataLogging(true);

            optionsBuilder.ConfigureWarnings(warnings =>
            {
                // Automatic client evaluation is no longer supported. This event is no longer generated
                //warnings.Throw(RelationalEventId.QueryClientEvaluationWarning);

                // When using .Include() in the EF query use one of these: .AsSplitQuery() or .AsSingleQuery
                // to remove the warning 'QuerySplittingBehavior' has been configured.
                // By default Entity Framework will use 'QuerySplittingBehavior.SingleQuery' which can potentially result in slow query performance. 
                warnings.Throw(RelationalEventId.MultipleCollectionIncludeWarning);
            });
#endif

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            if (IsAzureDB)
            {
                modelBuilder.HasServiceTier("Basic");
                modelBuilder.HasPerformanceLevel("Basic");
                modelBuilder.HasDatabaseMaxSize("2 GB");
            }

            #region Add configurations

            modelBuilder.ApplyConfiguration(new FlowGraphDescriptionEntityTypeConfiguration());

            #endregion

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
