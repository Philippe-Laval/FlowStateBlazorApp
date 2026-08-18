using FlowStateBlazor.Data.Data;

namespace FlowStateBlazor.Data.Tests.Builders
{
    /// <summary>
    /// Gestionnaire pour les ressources de test (nettoyage automatique).
    /// </summary>
    public class TestDatabaseFixture : IAsyncDisposable
    {
        private readonly FlowStateSqliteContext? _sqliteContext;
        private readonly FlowStateSqlServerContext? _sqlServerContext;

        public TestDatabaseFixture(FlowStateSqliteContext? sqliteContext = null, FlowStateSqlServerContext? sqlServerContext = null)
        {
            _sqliteContext = sqliteContext;
            _sqlServerContext = sqlServerContext;
        }

        public FlowStateContext GetContext()
        {
            return _sqliteContext ?? (_sqlServerContext as FlowStateContext) 
                ?? throw new InvalidOperationException("No context available");
        }

        public async ValueTask DisposeAsync()
        {
            if (_sqliteContext != null)
            {
                try
                {
                    await _sqliteContext.Database.EnsureDeletedAsync();
                }
                catch { }

                await _sqliteContext.DisposeAsync();
            }

            if (_sqlServerContext != null)
            {
                try
                {
                    await _sqlServerContext.Database.EnsureDeletedAsync();
                }
                catch { }

                await _sqlServerContext.DisposeAsync();
            }

            GC.SuppressFinalize(this);
        }
    }
}
