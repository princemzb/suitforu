-- =============================================
-- Migration: AddMessagingSystem
-- Date: 2025-12-11
-- Description: Ajout des tables Conversations et Messages pour la messagerie
-- =============================================

USE SuitForU;
GO

-- =============================================
-- Table: Conversations
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Conversations]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Conversations] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [GarmentId] UNIQUEIDENTIFIER NOT NULL,
        [User1Id] UNIQUEIDENTIFIER NOT NULL,
        [User2Id] UNIQUEIDENTIFIER NOT NULL,
        [LastMessageAt] DATETIME2 NULL,
        [LastMessageContent] NVARCHAR(500) NULL,
        [LastMessageSenderId] UNIQUEIDENTIFIER NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NULL,
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        
        CONSTRAINT FK_Conversations_Garments FOREIGN KEY ([GarmentId]) REFERENCES [dbo].[Garments]([Id]) ON DELETE NO ACTION,
        CONSTRAINT FK_Conversations_User1 FOREIGN KEY ([User1Id]) REFERENCES [dbo].[Users]([Id]) ON DELETE NO ACTION,
        CONSTRAINT FK_Conversations_User2 FOREIGN KEY ([User2Id]) REFERENCES [dbo].[Users]([Id]) ON DELETE NO ACTION
    );
    
    -- Index pour rechercher les conversations par utilisateur
    CREATE INDEX IX_Conversations_User1Id ON [dbo].[Conversations]([User1Id]);
    CREATE INDEX IX_Conversations_User2Id ON [dbo].[Conversations]([User2Id]);
    CREATE INDEX IX_Conversations_GarmentId ON [dbo].[Conversations]([GarmentId]);
    CREATE INDEX IX_Conversations_LastMessageAt ON [dbo].[Conversations]([LastMessageAt]);
    
    -- Index composite unique pour éviter les doublons
    CREATE UNIQUE INDEX IX_Conversations_GarmentId_Users ON [dbo].[Conversations]([GarmentId], [User1Id], [User2Id]);
    
    PRINT 'Table Conversations created successfully.';
END
GO

-- =============================================
-- Table: Messages
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Messages]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Messages] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [ConversationId] UNIQUEIDENTIFIER NOT NULL,
        [SenderId] UNIQUEIDENTIFIER NOT NULL,
        [Content] NVARCHAR(2000) NOT NULL,
        [IsRead] BIT NOT NULL DEFAULT 0,
        [ReadAt] DATETIME2 NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NULL,
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        
        CONSTRAINT FK_Messages_Conversations FOREIGN KEY ([ConversationId]) REFERENCES [dbo].[Conversations]([Id]) ON DELETE CASCADE,
        CONSTRAINT FK_Messages_Senders FOREIGN KEY ([SenderId]) REFERENCES [dbo].[Users]([Id]) ON DELETE NO ACTION
    );
    
    -- Index pour performance
    CREATE INDEX IX_Messages_ConversationId ON [dbo].[Messages]([ConversationId]);
    CREATE INDEX IX_Messages_SenderId ON [dbo].[Messages]([SenderId]);
    CREATE INDEX IX_Messages_CreatedAt ON [dbo].[Messages]([CreatedAt]);
    CREATE INDEX IX_Messages_IsRead ON [dbo].[Messages]([IsRead]);
    
    PRINT 'Table Messages created successfully.';
END
GO

-- =============================================
-- Insertion dans __EFMigrationsHistory
-- =============================================
IF NOT EXISTS (SELECT * FROM [dbo].[__EFMigrationsHistory] WHERE [MigrationId] = N'20251211_AddMessagingSystem')
BEGIN
    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251211_AddMessagingSystem', N'9.0.11');
    
    PRINT 'Migration AddMessagingSystem added to history.';
END
GO

PRINT '==============================================';
PRINT 'Migration AddMessagingSystem completed successfully!';
PRINT 'Tables: Conversations, Messages';
PRINT '==============================================';
GO
