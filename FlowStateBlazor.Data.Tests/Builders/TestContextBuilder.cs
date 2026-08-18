using FlowStateBlazor.Data.Data;
using Microsoft.EntityFrameworkCore;

namespace FlowStateBlazor.Data.Tests.Builders
{
    /// <summary>
    /// Builder pour créer des contextes DbContext pour les tests unitaires.
    /// Supporte SQLite (in-memory et fichier) et SQL Server.
    /// </summary>
    public class TestContextBuilder
    {
        private string? _sqliteConnectionString;
        private string? _sqlServerConnectionString;

        /// <summary>
        /// Crée une instance utilisant SQLite in-memory (défaut).
        /// </summary>
        public static TestContextBuilder UseSqliteInMemory()
        {
            return new TestContextBuilder { _sqliteConnectionString = "Data Source=:memory:" };
        }

        /// <summary>
        /// Crée une instance utilisant SQLite avec un fichier.
        /// </summary>
        public static TestContextBuilder UseSqliteFile(string filePath)
        {
            return new TestContextBuilder { _sqliteConnectionString = $"Data Source={filePath}" };
        }

        /// <summary>
        /// Crée une instance utilisant SQL Server.
        /// </summary>
        public static TestContextBuilder UseSqlServer(string connectionString)
        {
            return new TestContextBuilder { _sqlServerConnectionString = connectionString };
        }

        /// <summary>
        /// Crée un DbContext FlowStateSqliteContext pour les tests.
        /// </summary>
        public FlowStateSqliteContext BuildSqliteContext()
        {
            if (string.IsNullOrEmpty(_sqliteConnectionString))
                throw new InvalidOperationException("SQLite connection string not configured");

            var options = new DbContextOptionsBuilder<FlowStateSqliteContext>()
                .UseSqlite(_sqliteConnectionString)
                .Options;

            var context = new FlowStateSqliteContext(options);

            // Créer la base de données si elle n'existe pas
            context.Database.EnsureCreated();
            CreateTestTablesIfNeeded(context);

            return context;
        }

        /// <summary>
        /// Crée un DbContext FlowStateSqlServerContext pour les tests.
        /// </summary>
        public FlowStateSqlServerContext BuildSqlServerContext()
        {
            if (string.IsNullOrEmpty(_sqlServerConnectionString))
                throw new InvalidOperationException("SQL Server connection string not configured");

            var options = new DbContextOptionsBuilder<FlowStateSqlServerContext>()
                .UseSqlServer(_sqlServerConnectionString)
                .Options;

            var context = new FlowStateSqlServerContext(options);

            // Créer la base de données si elle n'existe pas
            context.Database.EnsureCreated();

            return context;
        }

        /// <summary>
        /// Crée les tables manuellement si EnsureCreated n'a pas fonctionné.
        /// </summary>
        private void CreateTestTablesIfNeeded(FlowStateContext context)
        {
            try
            {
                // Vérifier si la table existe
                var connection = context.Database.GetDbConnection();
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='FLOW_FLOWGRAPH_DESCRIPTION';";
                var result = command.ExecuteScalar();

                if (result == null)
                {
                    // Créer la table
                    var createSql = @"CREATE TABLE FLOW_FLOWGRAPH_DESCRIPTION (
                        ID INTEGER PRIMARY KEY AUTOINCREMENT,
                        NAME TEXT NOT NULL,
                        DESCRIPTION TEXT,
                        JSON_SERIALIZED_FLOW TEXT NOT NULL
                    );";
                    command.CommandText = createSql;
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
            catch
            {
                // Ignorer les erreurs de création
            }
        }

        /// <summary>
        /// Crée un DbContext avec les options génériques.
        /// </summary>
        public DbContextOptions<T> BuildOptions<T>(Action<DbContextOptionsBuilder<T>>? configure = null) 
            where T : FlowStateContext
        {
            var builder = new DbContextOptionsBuilder<T>();

            if (!string.IsNullOrEmpty(_sqliteConnectionString))
            {
                builder.UseSqlite(_sqliteConnectionString);
            }
            else if (!string.IsNullOrEmpty(_sqlServerConnectionString))
            {
                builder.UseSqlServer(_sqlServerConnectionString);
            }
            else
            {
                throw new InvalidOperationException("No database configured");
            }

            configure?.Invoke(builder);

            return builder.Options;
        }
    }
}
