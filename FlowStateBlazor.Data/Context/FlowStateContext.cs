using FlowStateBlazor.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FlowStateBlazor.Data.Data
{


    public partial class FlowStateContext : DbContext
    {
        public FlowStateContext()
        {
        }

        public FlowStateContext(DbContextOptions<FlowStateContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// This allows a sub class to call the base class 'DbContext' non typed constructor
        /// This is need because instances of the subclasses will use a specifc typed DbContextOptions
        /// which can not be converted into the parameter in the above constructor
        /// </summary>
        /// <param name="options"></param>
        protected FlowStateContext(DbContextOptions options)
            : base(options)
        {
        }

        #region DbSets
        public DbSet<FlowGraphDescription> FlowGraphDescriptions { get; set; }
        #endregion


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Base stuff common to every sql database
        }
    }
}
