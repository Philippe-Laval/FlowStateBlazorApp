using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using System.Reflection;

namespace FlowStateBlazor.Data.Configuration
{
    /// <summary>
    /// Gestionnaire centralisé des configurations de connexion à la base de données.
    /// Supporte plusieurs sources : User Secrets, appsettings.json, variables d'environnement.
    /// </summary>
    public class DatabaseConnectionConfiguration
    {
        private readonly IConfiguration _configuration;

        public DatabaseConnectionConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Crée et construit une configuration depuis les fichiers appsettings et User Secrets
        /// </summary>
        public static IConfiguration BuildConfiguration()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            var basePath = Directory.GetCurrentDirectory();

            var builder = new ConfigurationBuilder();

            // Ajouter les fichiers JSON directement avec les chemins relatifs
            var appSettingsPath = Path.Combine(basePath, "appsettings.json");
            if (File.Exists(appSettingsPath))
            {
                builder.AddJsonFile(appSettingsPath, optional: false, reloadOnChange: true);
            }

            var envAppSettingsPath = Path.Combine(basePath, $"appsettings.{env}.json");
            if (File.Exists(envAppSettingsPath))
            {
                builder.AddJsonFile(envAppSettingsPath, optional: true, reloadOnChange: true);
            }

            // Ajouter les variables d'environnement
            builder.AddEnvironmentVariables();

            // Ajouter User Secrets (pour développement)
            if (env == "Development")
            {
                try
                {
                    // Utiliser la réflexion pour obtenir l'assembly
                    var assembly = typeof(DatabaseConnectionConfiguration).Assembly;
                    builder.AddUserSecrets(assembly, optional: true);
                }
                catch
                {
                    // User Secrets non configuré, c'est OK
                }
            }

            return builder.Build();
        }

        /// <summary>
        /// Récupère la chaîne de connexion SQL Server
        /// Ordre de priorité : Env var > User Secrets > appsettings.{Environment}.json > appsettings.json
        /// </summary>
        public string GetSqlServerConnectionString()
        {
            return GetConnectionString("SqlServer", "FLOWSTATE_SQLSERVER_CONNECTION");
        }

        /// <summary>
        /// Récupère la chaîne de connexion Oracle
        /// </summary>
        public string GetOracleConnectionString()
        {
            return GetConnectionString("Oracle", "FLOWSTATE_ORACLE_CONNECTION");
        }

        /// <summary>
        /// Récupère la chaîne de connexion SQLite
        /// </summary>
        public string GetSqliteConnectionString()
        {
            return GetConnectionString("Sqlite", "FLOWSTATE_SQLITE_CONNECTION");
        }

        /// <summary>
        /// Méthode utilitaire pour récupérer une chaîne de connexion
        /// Priorité : Variable d'environnement > Configuration (appsettings, User Secrets)
        /// </summary>
        private string GetConnectionString(string configKey, string environmentVariableName)
        {
            // 1. Essayer la variable d'environnement (production/CI-CD) - priorité maximale
            var envConnection = Environment.GetEnvironmentVariable(environmentVariableName);
            if (!string.IsNullOrEmpty(envConnection))
                return envConnection;

            // 2. Essayer la configuration (appsettings, User Secrets)
            var configConnection = _configuration.GetConnectionString(configKey);
            if (!string.IsNullOrEmpty(configConnection))
                return configConnection;

            throw new InvalidOperationException(
                $"Chaîne de connexion introuvable pour '{configKey}'. " +
                $"Configurez via : Env var '{environmentVariableName}', User Secrets, ou appsettings.json");
        }
    }
}
