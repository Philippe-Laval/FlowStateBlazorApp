IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818093440_InitialCreate'
)
BEGIN
    CREATE TABLE [FLOW_FLOWGRAPH_DESCRIPTION] (
        [ID] int NOT NULL IDENTITY,
        [NAME] nvarchar(255) NOT NULL,
        [DESCRIPTION] nvarchar(255) NOT NULL,
        [JSON_SERIALIZED_FLOW] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_FLOW_FLOWGRAPH_DESCRIPTION] PRIMARY KEY ([ID])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260818093440_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260818093440_InitialCreate', N'10.0.11');
END;

COMMIT;
GO

