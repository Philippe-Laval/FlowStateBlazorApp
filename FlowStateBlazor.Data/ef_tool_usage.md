# Using dotnet ef Command-Line Tool

The dotnet ef command-line tool is a powerful utility for managing Entity Framework Core (EF Core) tasks such as creating and applying migrations, generating code for models, and more. This tool is an extension of the dotnet command and is part of the .NET Core SDK.

## Installation

To install dotnet ef as a global tool, use the following command:

```
dotnet tool install --global dotnet-ef
```

To update the tool, use:

```
dotnet tool update --global dotnet-ef
```

Before using the tool on a specific project, add the Microsoft.EntityFrameworkCore.Design package:

```
dotnet add package Microsoft.EntityFrameworkCore.Design
```

Verify the installation by running:

```
dotnet ef
```

This command should display the version of the EF Core CLI tools.

## Common Commands

```
cd C:\CodeGithubPerso\FlowStateBlazorApp\FlowStateBlazor.Data
```

### Creating and Applying Migrations

To add a new migration, use:

```
dotnet ef migrations add <MigrationName>
```

To update the database to the latest migration, use:

```
dotnet ef database update
```

To update the database to a specific migration, specify the migration name:

```
dotnet ef database update <MigrationName>
```

Managing the DbContext

To get information about a DbContext type, use:

```
dotnet ef dbcontext info
```

To list available DbContext types, use:

```
dotnet ef dbcontext list
```

To generate a compiled version of the model used by the DbContext, use:

```
dotnet ef dbcontext optimize
```

Scaffolding

To generate code for a DbContext and entity types from an existing database, use:

```
dotnet ef dbcontext scaffold "<ConnectionString>" <Provider>
```

For example, to scaffold a SQL Server database:

```
dotnet ef dbcontext scaffold "Server=(localdb)\\mssqllocaldb;Database=Blogging;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -o Models
```

Generating SQL Scripts

To generate a SQL script from the DbContext, bypassing any migrations, use:

```
dotnet ef dbcontext script
```

To generate a SQL script from migrations, use:

```
dotnet ef migrations script
```

Additional Commands

Drop Database: 
```
dotnet ef database drop
```

List Migrations: 
```
dotnet ef migrations list
```

Remove Last Migration:
```
dotnet ef migrations remove
```

Check Pending Model Changes:
```
dotnet ef migrations has-pending-model-changes
```

# Important Considerations

When using dotnet ef, ensure that the target project and startup project are correctly specified. 
The target project is where the commands add or remove files, 
and the startup project is the one that the tools build and run. 
By default, the project in the current directory is used, but you can specify different projects using the --project and --startup-project options.

By following these guidelines, you can effectively manage your EF Core projects using the dotnet ef command-line tool.