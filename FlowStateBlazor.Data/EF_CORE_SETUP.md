# EF Core SQL Server Setup

## Configuration dans appsettings.json

Ajoutez la chaîne de connexion SQL Server dans votre `appsettings.json` :

```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Server=YOUR_SERVER;Database=FlowStateDb;Trusted_Connection=true;Encrypt=false;"
  }
}
```

Ou pour Azure SQL :
```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Server=tcp:YOUR_SERVER.database.windows.net,1433;Initial Catalog=FlowStateDb;Persist Security Info=False;User ID=YOUR_USER;Password=YOUR_PASSWORD;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }
}
```

## Configuration dans Program.cs

Injectez le DbContext dans les services (dans votre projet API ou Blazor Web App) :

```csharp
using FlowStateBlazor.Data.Data;

// Ajouter le DbContext
builder.Services.AddDbContext<FlowStateDbContext>(options =>
	options.UseSqlServer(
		builder.Configuration.GetConnectionString("DefaultConnection"),
		sqlOptions => sqlOptions.MigrationsAssembly("FlowStateBlazor.Data")));
```

## Créer les migrations

Depuis le Package Manager Console ou CLI :

```powershell
# Package Manager Console
Add-Migration InitialCreate -Project FlowStateBlazor.Data
Update-Database -Project FlowStateBlazor.Data
```

Ou avec dotnet CLI :

```bash
dotnet ef migrations add InitialCreate --project FlowStateBlazor.Data
dotnet ef database update --project FlowStateBlazor.Data
```

## Structure de la base de données

La migration créera la table `FlowGraphDescriptions` avec :
- `Id`: INT, PRIMARY KEY, Identity
- `Name`: NVARCHAR(255), NOT NULL
- `FlowSerialized`: NVARCHAR(MAX), NOT NULL

## Utilisation

```csharp
using FlowStateBlazor.Data.Data;
using FlowStateBlazor.Data.Models;

// Injection du DbContext
public class MyService
{
	private readonly FlowStateDbContext _context;

	public MyService(FlowStateDbContext context)
	{
		_context = context;
	}

	// Créer
	public async Task CreateFlowGraphAsync(string name, string flowSerialized)
	{
		var flow = new FlowGraphDescription 
		{ 
			Name = name, 
			FlowSerialized = flowSerialized 
		};
		_context.FlowGraphDescriptions.Add(flow);
		await _context.SaveChangesAsync();
	}

	// Lire
	public async Task<FlowGraphDescription?> GetFlowGraphAsync(int id)
	{
		return await _context.FlowGraphDescriptions.FindAsync(id);
	}

	// Lister tout
	public async Task<List<FlowGraphDescription>> GetAllFlowGraphsAsync()
	{
		return await _context.FlowGraphDescriptions.ToListAsync();
	}

	// Mettre à jour
	public async Task UpdateFlowGraphAsync(int id, string name, string flowSerialized)
	{
		var flow = await _context.FlowGraphDescriptions.FindAsync(id);
		if (flow != null)
		{
			flow.Name = name;
			flow.FlowSerialized = flowSerialized;
			_context.FlowGraphDescriptions.Update(flow);
			await _context.SaveChangesAsync();
		}
	}

	// Supprimer
	public async Task DeleteFlowGraphAsync(int id)
	{
		var flow = await _context.FlowGraphDescriptions.FindAsync(id);
		if (flow != null)
		{
			_context.FlowGraphDescriptions.Remove(flow);
			await _context.SaveChangesAsync();
		}
	}
}
```
