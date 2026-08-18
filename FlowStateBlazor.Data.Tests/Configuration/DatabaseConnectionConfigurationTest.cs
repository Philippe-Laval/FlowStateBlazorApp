using FlowStateBlazor.Data.Configuration;
using Microsoft.Extensions.Configuration;

namespace FlowStateBlazor.Data.Tests.Configuration
{
    [TestClass]
    [DoNotParallelize]
    public sealed class DatabaseConnectionConfigurationTest
    {
        private IConfiguration? _configuration;
        private DatabaseConnectionConfiguration? _dbConfig;

        [TestInitialize]
        public void Setup()
        {
            // Test DB
            var configDict = new Dictionary<string, string?>
            {
                { "ConnectionStrings:SqlServer", "Server=localhost;Database=TestDb;" },
                { "ConnectionStrings:Oracle", "Data Source=localhost:1521/test;" },
                { "ConnectionStrings:Sqlite", "Data Source=test.db" }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configDict)
                .Build();

            _dbConfig = new DatabaseConnectionConfiguration(_configuration);

            Environment.SetEnvironmentVariable("FLOWSTATE_SQLSERVER_CONNECTION", null);
            Environment.SetEnvironmentVariable("FLOWSTATE_ORACLE_CONNECTION", null);
            Environment.SetEnvironmentVariable("FLOWSTATE_SQLITE_CONNECTION", null);
        }

        [TestCleanup]
        public void Cleanup()
        {
            Environment.SetEnvironmentVariable("FLOWSTATE_SQLSERVER_CONNECTION", null);
            Environment.SetEnvironmentVariable("FLOWSTATE_ORACLE_CONNECTION", null);
            Environment.SetEnvironmentVariable("FLOWSTATE_SQLITE_CONNECTION", null);
        }

        #region GetSqlServerConnectionString Tests

        [TestMethod]
        [Description("Retourne la chaîne depuis appsettings")]
        public void GetSqlServerConnectionString_ReturnsFromConfiguration_WhenNoEnvironmentVariable()
        {
            Environment.SetEnvironmentVariable("FLOWSTATE_SQLSERVER_CONNECTION", null);
            var result = _dbConfig!.GetSqlServerConnectionString();
            Assert.AreEqual("Server=localhost;Database=TestDb;", result);
        }

        [TestMethod]
        [Description("Privilégie la variable d'environnement")]
        public void GetSqlServerConnectionString_ReturnsFromEnvironmentVariable_WhenSet()
        {
            var envConnection = "Server=prod-server;Database=ProdDb;";
            Environment.SetEnvironmentVariable("FLOWSTATE_SQLSERVER_CONNECTION", envConnection);

            var result = _dbConfig!.GetSqlServerConnectionString();
            Assert.AreEqual(envConnection, result);
        }

        [TestMethod]
        [Description("Lève une exception si pas de configuration")]
        public void GetSqlServerConnectionString_ThrowsException_WhenNoConnectionString()
        {
            var emptyConfig = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>())
                .Build();
            var dbConfig = new DatabaseConnectionConfiguration(emptyConfig);
            Environment.SetEnvironmentVariable("FLOWSTATE_SQLSERVER_CONNECTION", null);

            try
            {
                dbConfig.GetSqlServerConnectionString();
                Assert.Fail("Exception attendue");
            }
            catch (InvalidOperationException)
            {
                // OK
            }
        }

        #endregion

        #region GetOracleConnectionString Tests

        [TestMethod]
        [Description("Retourne la chaîne depuis appsettings")]
        public void GetOracleConnectionString_ReturnsFromConfiguration_WhenNoEnvironmentVariable()
        {
            Environment.SetEnvironmentVariable("FLOWSTATE_ORACLE_CONNECTION", null);
            var result = _dbConfig!.GetOracleConnectionString();
            Assert.AreEqual("Data Source=localhost:1521/test;", result);
        }

        [TestMethod] 
        [Description("Privilégie la variable d'environnement")]
        public void GetOracleConnectionString_ReturnsFromEnvironmentVariable_WhenSet()
        {
            var envConnection = "Data Source=oracleserver:1521/PROD;";
            Environment.SetEnvironmentVariable("FLOWSTATE_ORACLE_CONNECTION", envConnection);

            var result = _dbConfig!.GetOracleConnectionString();
            Assert.AreEqual(envConnection, result);
        }

        [TestMethod]
        [Description("Lève une exception si pas de configuration")]
        public void GetOracleConnectionString_ThrowsException_WhenNoConnectionString()
        {
            var emptyConfig = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>())
                .Build();
            var dbConfig = new DatabaseConnectionConfiguration(emptyConfig);
            Environment.SetEnvironmentVariable("FLOWSTATE_ORACLE_CONNECTION", null);

            try
            {
                dbConfig.GetOracleConnectionString();
                Assert.Fail("Exception attendue");
            }
            catch (InvalidOperationException)
            {
                // OK
            }
        }

        #endregion

        #region GetSqliteConnectionString Tests

        [TestMethod]
        [Description("Retourne la chaîne depuis appsettings")]
        public void GetSqliteConnectionString_ReturnsFromConfiguration_WhenNoEnvironmentVariable()
        {
            Environment.SetEnvironmentVariable("FLOWSTATE_SQLITE_CONNECTION", null);
            var result = _dbConfig!.GetSqliteConnectionString();
            Assert.AreEqual("Data Source=test.db", result);
        }

        [TestMethod]
        [Description("Privilégie la variable d'environnement")]
        public void GetSqliteConnectionString_ReturnsFromEnvironmentVariable_WhenSet()
        {
            var envConnection = "Data Source=/data/production.db";
            Environment.SetEnvironmentVariable("FLOWSTATE_SQLITE_CONNECTION", envConnection);

            var result = _dbConfig!.GetSqliteConnectionString();
            Assert.AreEqual(envConnection, result);
        }

        [TestMethod]
        [Description("Lève une exception si pas de configuration")]
        public void GetSqliteConnectionString_ThrowsException_WhenNoConnectionString()
        {
            var emptyConfig = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>())
                .Build();
            var dbConfig = new DatabaseConnectionConfiguration(emptyConfig);
            Environment.SetEnvironmentVariable("FLOWSTATE_SQLITE_CONNECTION", null);

            try
            {
                dbConfig.GetSqliteConnectionString();
                Assert.Fail("Exception attendue");
            }
            catch (InvalidOperationException)
            {
                // OK
            }
        }

        #endregion

        #region Priority Tests

        [TestMethod]
        [Description("Privilégie la variable d'environnement sur la configuration")]
        public void ConnectionString_FavorsEnvironmentVariable_OverConfiguration()
        {
            var envConnection = "Server=prod-server;Database=ProdDb;";
            Environment.SetEnvironmentVariable("FLOWSTATE_SQLSERVER_CONNECTION", envConnection);

            var result = _dbConfig!.GetSqlServerConnectionString();
            Assert.AreEqual(envConnection, result);
        }

        [TestMethod]
        [Description("Utilise la configuration sans variable d'environnement")]
        public void ConnectionString_UsesConfiguration_WhenNoEnvironmentVariable()
        {
            Environment.SetEnvironmentVariable("FLOWSTATE_SQLSERVER_CONNECTION", null);
            var result = _dbConfig!.GetSqlServerConnectionString();
            Assert.AreEqual("Server=localhost;Database=TestDb;", result);
        }

        #endregion

        #region Error Message Tests

        [TestMethod]
        [Description("Message d'erreur contient la clé de configuration")]
        public void ExceptionMessage_ContainsConfigKey_WhenConnectionStringNotFound()
        {
            var emptyConfig = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>())
                .Build();
            var dbConfig = new DatabaseConnectionConfiguration(emptyConfig);
            Environment.SetEnvironmentVariable("FLOWSTATE_SQLSERVER_CONNECTION", null);

            try
            {
                dbConfig.GetSqlServerConnectionString();
                Assert.Fail("Exception attendue");
            }
            catch (InvalidOperationException ex)
            {
                Assert.IsTrue(ex.Message.Contains("SqlServer"));
                Assert.IsTrue(ex.Message.Contains("FLOWSTATE_SQLSERVER_CONNECTION"));
            }
        }

        [TestMethod]
        [Description("Message d'erreur contient la variable d'environnement")]
        public void ExceptionMessage_ContainsEnvironmentVariableName_WhenConnectionStringNotFound()
        {
            var emptyConfig = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>())
                .Build();
            var dbConfig = new DatabaseConnectionConfiguration(emptyConfig);
            Environment.SetEnvironmentVariable("FLOWSTATE_ORACLE_CONNECTION", null);

            try
            {
                dbConfig.GetOracleConnectionString();
                Assert.Fail("Exception attendue");
            }
            catch (InvalidOperationException ex)
            {
                Assert.IsTrue(ex.Message.Contains("FLOWSTATE_ORACLE_CONNECTION"));
            }
        }

        #endregion

        #region Empty/Null Values Tests

        [TestMethod]
        [Description("Chaînes vides traitées comme manquantes")]
        public void ConnectionString_ThrowsException_WhenEmptyString()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> { { "ConnectionStrings:SqlServer", "" } })
                .Build();
            var dbConfig = new DatabaseConnectionConfiguration(config);
            Environment.SetEnvironmentVariable("FLOWSTATE_SQLSERVER_CONNECTION", null);

            try
            {
                dbConfig.GetSqlServerConnectionString();
                Assert.Fail("Exception attendue");
            }
            catch (InvalidOperationException)
            {
                // OK
            }
        }

        [TestMethod]
        [Description("Ignores les variables d'environnement vides")]
        public void ConnectionString_UsesConfiguration_WhenEnvironmentVariableIsEmpty()
        {
            Environment.SetEnvironmentVariable("FLOWSTATE_SQLSERVER_CONNECTION", "");
            var result = _dbConfig!.GetSqlServerConnectionString();
            Assert.AreEqual("Server=localhost;Database=TestDb;", result);
        }

        [TestMethod]
        [Description("Ignores les variables d'environnement null")]
        public void ConnectionString_UsesConfiguration_WhenEnvironmentVariableIsNull()
        {
            Environment.SetEnvironmentVariable("FLOWSTATE_SQLSERVER_CONNECTION", null);
            var result = _dbConfig!.GetSqlServerConnectionString();
            Assert.AreEqual("Server=localhost;Database=TestDb;", result);
        }

        #endregion

        #region Multiple Databases Test

        [TestMethod]
        [Description("Trois types de bases de données configurés indépendamment")]
        public void AllDatabaseTypes_CanBeConfiguredIndependently()
        {
            var sqlResult = _dbConfig!.GetSqlServerConnectionString();
            var oracleResult = _dbConfig.GetOracleConnectionString();
            var sqliteResult = _dbConfig.GetSqliteConnectionString();

            Assert.AreEqual("Server=localhost;Database=TestDb;", sqlResult);
            Assert.AreEqual("Data Source=localhost:1521/test;", oracleResult);
            Assert.AreEqual("Data Source=test.db", sqliteResult);
        }

        #endregion

        #region Partial Configuration Tests

        [TestMethod]
        [Description("Peut avoir uniquement SQL Server configuré")]
        public void PartialConfiguration_SqlServerOnly()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> { { "ConnectionStrings:SqlServer", "Server=localhost;" } })
                .Build();
            var dbConfig = new DatabaseConnectionConfiguration(config);

            var result = dbConfig.GetSqlServerConnectionString();
            Assert.AreEqual("Server=localhost;", result);

            try
            {
                dbConfig.GetOracleConnectionString();
                Assert.Fail("Exception attendue");
            }
            catch (InvalidOperationException)
            {
                // OK
            }
        }

        [TestMethod]
        [Description("Peut avoir uniquement Oracle configuré")]
        public void PartialConfiguration_OracleOnly()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> { { "ConnectionStrings:Oracle", "Data Source=localhost;" } })
                .Build();
            var dbConfig = new DatabaseConnectionConfiguration(config);

            var result = dbConfig.GetOracleConnectionString();
            Assert.AreEqual("Data Source=localhost;", result);

            try
            {
                dbConfig.GetSqlServerConnectionString();
                Assert.Fail("Exception attendue");
            }
            catch (InvalidOperationException)
            {
                // OK
            }
        }

        #endregion
    }
}
