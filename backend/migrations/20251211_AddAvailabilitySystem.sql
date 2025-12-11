-- =============================================
-- Migration: AddAvailabilitySystem
-- Date: 2025-12-11
-- Description: Ajout de la table GarmentAvailabilities pour gérer le calendrier de disponibilité
-- =============================================

USE SuitForU;
GO

-- =============================================
-- Table: GarmentAvailabilities
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GarmentAvailabilities]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[GarmentAvailabilities] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [GarmentId] UNIQUEIDENTIFIER NOT NULL,
        [Date] DATE NOT NULL,
        [IsAvailable] BIT NOT NULL DEFAULT 1,
        [BlockedReason] INT NULL,
        [RentalId] UNIQUEIDENTIFIER NULL,
        [Notes] NVARCHAR(500) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NULL,
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        
        CONSTRAINT FK_GarmentAvailabilities_Garments FOREIGN KEY ([GarmentId]) REFERENCES [dbo].[Garments]([Id]) ON DELETE CASCADE,
        CONSTRAINT FK_GarmentAvailabilities_Rentals FOREIGN KEY ([RentalId]) REFERENCES [dbo].[Rentals]([Id]) ON DELETE SET NULL
    );
    
    -- Index pour performance
    CREATE INDEX IX_GarmentAvailabilities_GarmentId ON [dbo].[GarmentAvailabilities]([GarmentId]);
    CREATE INDEX IX_GarmentAvailabilities_Date ON [dbo].[GarmentAvailabilities]([Date]);
    
    -- Index unique pour éviter les doublons (1 seule entrée par garment + date)
    CREATE UNIQUE INDEX IX_GarmentAvailabilities_GarmentId_Date_Unique ON [dbo].[GarmentAvailabilities]([GarmentId], [Date]);
    
    PRINT 'Table GarmentAvailabilities created successfully.';
END
GO

-- =============================================
-- Insertion dans __EFMigrationsHistory
-- =============================================
IF NOT EXISTS (SELECT * FROM [dbo].[__EFMigrationsHistory] WHERE [MigrationId] = N'20251211_AddAvailabilitySystem')
BEGIN
    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251211_AddAvailabilitySystem', N'9.0.11');
    
    PRINT 'Migration AddAvailabilitySystem added to history.';
END
GO

PRINT '==============================================';
PRINT 'Migration AddAvailabilitySystem completed successfully!';
PRINT 'Table: GarmentAvailabilities';
PRINT '==============================================';
GO
