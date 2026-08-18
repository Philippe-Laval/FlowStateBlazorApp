using FlowStateBlazor.Data.EntityTypeConfigurations.Oracle;
using Microsoft.EntityFrameworkCore;

namespace FlowStateBlazor.Data.Data
{

    public partial class FlowStateOracleContext : FlowStateContext
    {
        public FlowStateOracleContext(DbContextOptions<FlowStateOracleContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=127.0.0.1)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=XEPDB1)));User ID=DEMO;Password=demo
                optionsBuilder.UseOracle(@"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=127.0.0.1)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=XEPDB1)));User ID=FLOWSTATE;Password=rse3");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Add configurations

            modelBuilder.ApplyConfiguration(new FlowGraphDescriptionEntityTypeConfiguration());

            #endregion

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
