# Guide de configuration sécurisée des secrets EF Core

## 🔒 Principes de sécurité

Vous ne devez **JAMAIS** coder en dur les informations sensibles comme :
- Les mots de passe
- Les identifiants de base de données
- Les connexions de production

Cette implémentation suit les **meilleures pratiques Microsoft** en utilisant une hiérarchie de priorités.

## 📋 Hiérarchie de configuration (ordre de priorité)

1. **Variables d'environnement** (Priorité MAXIMUM - Production/CI-CD)
2. **User Secrets** (Développement local sécurisé)
3. **appsettings.{Environment}.json** (Configuration par environnement)
4. **appsettings.json** (Configuration par défaut)

## 🚀 Configuration pour développement local

### Étape 1 : Configurer User Secrets

Pour éviter de stocker les secrets en texte clair, utilisez **User Secrets** :

```bash
# Initialiser User Secrets pour le projet
cd C:\CodeGithubPerso\FlowStateBlazorApp
dotnet user-secrets init --project FlowStateBlazor.Data

# Ajouter les chaînes de connexion
dotnet user-secrets set "ConnectionStrings:SqlServer" "Server=localhost;Database=FLOWSTATE;User Id=sa;Password=YourPassword;TrustServerCertificate=True;" --project FlowStateBlazor.Data

dotnet user-secrets set "ConnectionStrings:Oracle" "Data Source=localhost:1521/orcl;User Id=admin;Password=YourPassword;" --project FlowStateBlazor.Data

dotnet user-secrets set "ConnectionStrings:Sqlite" "Data Source=FLOWSTATE.db" --project FlowStateBlazor.Data
```

Les secrets sont stockés **de manière sécurisée** par le système d'exploitation (pas dans le code!).

### Étape 2 : Fichier appsettings.json (sans secrets)

```json
{
  "ConnectionStrings": {
	"SqlServer": "REMOVED_FOR_SECURITY",
	"Oracle": "REMOVED_FOR_SECURITY",
	"Sqlite": "Data Source=FLOWSTATE.db"
  }
}
```

### Étape 3 : Ajouter au .gitignore

Assurez-vous que vos fichiers sensibles ne sont pas commités :

```
# Fichiers secrets locaux
appsettings.*.json
secrets.json
*.db
```

## 🔧 Utilisation des migrations

### SQL Server
```bash
dotnet ef migrations add InitialCreate --context FlowStateSqlServerContext --output-dir Migrations/SqlServerMigrations
dotnet ef database update --context FlowStateSqlServerContext
```

### Oracle
```bash
dotnet ef migrations add InitialCreate --context FlowStateOracleContext --output-dir Migrations/OracleMigrations
dotnet ef database update --context FlowStateOracleContext
```

### SQLite
```bash
dotnet ef migrations add InitialCreate --context FlowStateSqliteContext --output-dir Migrations/SqliteMigrations
dotnet ef database update --context FlowStateSqliteContext
```

## 🌍 Configuration pour Production

En production, utilisez des **variables d'environnement** :

```bash
# Exemple avec PowerShell
$env:FLOWSTATE_SQLSERVER_CONNECTION = "Server=prod-server;Database=FLOWSTATE;..."
$env:FLOWSTATE_ORACLE_CONNECTION = "Data Source=prod-oracle;..."
$env:FLOWSTATE_SQLITE_CONNECTION = "Data Source=/data/flowstate.db"
```

Ou via Docker :
```dockerfile
ENV FLOWSTATE_SQLSERVER_CONNECTION="Server=dbserver;Database=FLOWSTATE;..."
ENV FLOWSTATE_ORACLE_CONNECTION="Data Source=oracleserver;..."
```

Ou via Azure Key Vault :
```csharp
// Dans Program.cs
var keyVaultUrl = new Uri($"https://{keyVaultName}.vault.azure.net/");
var credential = new DefaultAzureCredential();
builder.Configuration.AddAzureKeyVault(keyVaultUrl, credential);
```

## 📝 Variables d'environnement supportées

- `FLOWSTATE_SQLSERVER_CONNECTION` - Chaîne de connexion SQL Server
- `FLOWSTATE_ORACLE_CONNECTION` - Chaîne de connexion Oracle
- `FLOWSTATE_SQLITE_CONNECTION` - Chemin de la base SQLite

## ⚠️ Avertissements de sécurité

❌ NE PAS :
- Coder en dur les secrets avec les credentials réelles
- Commiter les fichiers `appsettings.*.json` contenant des secrets
- Partager vos secrets User Secrets
- Utiliser le même mot de passe pour tous les environnements

✔️ À FAIRE :
- Utiliser des secrets différents par environnement
- Rotationner les palabras passe régulièrement
- Auditer l'accès aux bases de données
- Utiliser des outils comme Azure Key Vault en production

## Dépannage

### "Chaîne de connexion introuvable"

Vérifiez que la variable d'environnement ou le User Secret est configuré :

```bash
# Vérifier les secrets stockés
dotnet user-secrets list --project FlowStateBlazor.Data
```

### Migrations depuis un répertoire différent

Si vous exécutez dotnet ef depuis un autre répertoire, utilisez `-p` :

```bash
dotnet ef migrations add InitialCreate --project FlowStateBlazor.Data -p FlowStateBlazor.Data
```
