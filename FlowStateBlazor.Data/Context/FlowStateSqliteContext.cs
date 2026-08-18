using FlowStateBlazor.Data.EntityTypeConfigurations.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace FlowStateBlazor.Data.Data
{
    public partial class FlowStateSqliteContext : FlowStateContext
    {
        public FlowStateSqliteContext(DbContextOptions<FlowStateSqliteContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite(@"Data Source=FLOWSTATE.db");
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
