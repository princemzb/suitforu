-- =============================================
-- SuitForU Database Creation Script
-- Generated: 2025-12-11
-- Description: Script complet de création de la base de données
-- =============================================

USE master;
GO

-- Créer la base de données si elle n'existe pas
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'SuitForU')
BEGIN
    CREATE DATABASE SuitForU;
    PRINT 'Database SuitForU created successfully.';
END
ELSE
BEGIN
    PRINT 'Database SuitForU already exists.';
END
GO

USE SuitForU;
GO

-- =============================================
-- Table: Users
-- Description: Utilisateurs de la plateforme (locataires et propriétaires)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Users] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [Email] NVARCHAR(256) NOT NULL UNIQUE,
        [PasswordHash] NVARCHAR(MAX) NOT NULL,
        [FirstName] NVARCHAR(100) NOT NULL,
        [LastName] NVARCHAR(100) NOT NULL,
        [PhoneNumber] NVARCHAR(20) NULL,
        [Address] NVARCHAR(500) NULL,
        [City] NVARCHAR(100) NULL,
        [PostalCode] NVARCHAR(20) NULL,
        [Country] NVARCHAR(100) NULL DEFAULT 'France',
        [ProfilePictureUrl] NVARCHAR(500) NULL,
        [Bio] NVARCHAR(MAX) NULL,
        [IsEmailConfirmed] BIT NOT NULL DEFAULT 0,
        [EmailConfirmationToken] NVARCHAR(MAX) NULL,
        [AuthProvider] NVARCHAR(50) NULL,
        [ExternalAuthId] NVARCHAR(256) NULL,
        [Rating] DECIMAL(3,2) NULL,
        [RatingCount] INT NOT NULL DEFAULT 0,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [IsDeleted] BIT NOT NULL DEFAULT 0
    );
    
    CREATE INDEX IX_Users_Email ON [dbo].[Users]([Email]);
    CREATE INDEX IX_Users_CreatedAt ON [dbo].[Users]([CreatedAt]);
    PRINT 'Table Users created successfully.';
END
GO

-- =============================================
-- Table: RefreshTokens
-- Description: Tokens de rafraîchissement JWT avec rotation
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RefreshTokens]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[RefreshTokens] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [Token] NVARCHAR(500) NOT NULL UNIQUE,
        [UserId] UNIQUEIDENTIFIER NOT NULL,
        [ExpiresAt] DATETIME2 NOT NULL,
        [IsRevoked] BIT NOT NULL DEFAULT 0,
        [RevokedAt] DATETIME2 NULL,
        [CreatedByIp] NVARCHAR(50) NULL,
        [RevokedByIp] NVARCHAR(50) NULL,
        [ReplacedByToken] NVARCHAR(500) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_RefreshTokens_Users FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id]) ON DELETE CASCADE
    );
    
    CREATE INDEX IX_RefreshTokens_Token ON [dbo].[RefreshTokens]([Token]);
    CREATE INDEX IX_RefreshTokens_UserId ON [dbo].[RefreshTokens]([UserId]);
    CREATE INDEX IX_RefreshTokens_ExpiresAt ON [dbo].[RefreshTokens]([ExpiresAt]);
    PRINT 'Table RefreshTokens created successfully.';
END
GO

-- =============================================
-- Table: Garments
-- Description: Vêtements de cérémonie disponibles à la location
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Garments]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Garments] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [OwnerId] UNIQUEIDENTIFIER NOT NULL,
        [Title] NVARCHAR(200) NOT NULL,
        [Description] NVARCHAR(MAX) NOT NULL,
        [Type] INT NOT NULL, -- 0=Suit, 1=Dress, 2=Tuxedo, 3=Shirt, 4=Pants, 5=Shoes, 6=Accessories
        [Condition] INT NOT NULL, -- 0=New, 1=LikeNew, 2=Good, 3=Fair
        [Size] NVARCHAR(10) NOT NULL,
        [Brand] NVARCHAR(100) NULL,
        [Color] NVARCHAR(50) NULL,
        [DailyPrice] DECIMAL(18,2) NOT NULL,
        [DepositAmount] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [PickupAddress] NVARCHAR(500) NOT NULL,
        [City] NVARCHAR(100) NOT NULL,
        [PostalCode] NVARCHAR(20) NOT NULL,
        [Country] NVARCHAR(100) NOT NULL DEFAULT 'France',
        [IsAvailable] BIT NOT NULL DEFAULT 1,
        [ViewCount] INT NOT NULL DEFAULT 0,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        CONSTRAINT FK_Garments_Users FOREIGN KEY ([OwnerId]) REFERENCES [dbo].[Users]([Id])
    );
    
    CREATE INDEX IX_Garments_OwnerId ON [dbo].[Garments]([OwnerId]);
    CREATE INDEX IX_Garments_City ON [dbo].[Garments]([City]);
    CREATE INDEX IX_Garments_Type ON [dbo].[Garments]([Type]);
    CREATE INDEX IX_Garments_DailyPrice ON [dbo].[Garments]([DailyPrice]);
    CREATE INDEX IX_Garments_IsAvailable ON [dbo].[Garments]([IsAvailable]);
    CREATE INDEX IX_Garments_CreatedAt ON [dbo].[Garments]([CreatedAt]);
    PRINT 'Table Garments created successfully.';
END
GO

-- =============================================
-- Table: GarmentImages
-- Description: Images des vêtements (max 3 par vêtement)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GarmentImages]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[GarmentImages] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [GarmentId] UNIQUEIDENTIFIER NOT NULL,
        [ImageUrl] NVARCHAR(500) NOT NULL,
        [IsPrimary] BIT NOT NULL DEFAULT 0,
        [DisplayOrder] INT NOT NULL DEFAULT 0,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        CONSTRAINT FK_GarmentImages_Garments FOREIGN KEY ([GarmentId]) REFERENCES [dbo].[Garments]([Id]) ON DELETE CASCADE
    );
    
    CREATE INDEX IX_GarmentImages_GarmentId ON [dbo].[GarmentImages]([GarmentId]);
    PRINT 'Table GarmentImages created successfully.';
END
GO

-- =============================================
-- Table: Rentals
-- Description: Réservations de vêtements
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Rentals]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Rentals] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [GarmentId] UNIQUEIDENTIFIER NOT NULL,
        [RenterId] UNIQUEIDENTIFIER NOT NULL,
        [OwnerId] UNIQUEIDENTIFIER NOT NULL,
        [StartDate] DATETIME2 NOT NULL,
        [EndDate] DATETIME2 NOT NULL,
        [DurationDays] INT NOT NULL,
        [DailyPrice] DECIMAL(18,2) NOT NULL,
        [TotalPrice] DECIMAL(18,2) NOT NULL,
        [DepositAmount] DECIMAL(18,2) NOT NULL,
        [Status] INT NOT NULL DEFAULT 0, -- 0=Pending, 1=OwnerAccepted, 2=Confirmed, 3=Active, 4=Completed, 5=Cancelled, 6=Disputed
        [OwnerAcceptedAt] DATETIME2 NULL,
        [RenterConfirmedAt] DATETIME2 NULL,
        [PickupConfirmedAt] DATETIME2 NULL,
        [ReturnConfirmedAt] DATETIME2 NULL,
        [CancellationReason] NVARCHAR(500) NULL,
        [CancelledAt] DATETIME2 NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        CONSTRAINT FK_Rentals_Garments FOREIGN KEY ([GarmentId]) REFERENCES [dbo].[Garments]([Id]),
        CONSTRAINT FK_Rentals_Renters FOREIGN KEY ([RenterId]) REFERENCES [dbo].[Users]([Id]),
        CONSTRAINT FK_Rentals_Owners FOREIGN KEY ([OwnerId]) REFERENCES [dbo].[Users]([Id])
    );
    
    CREATE INDEX IX_Rentals_GarmentId ON [dbo].[Rentals]([GarmentId]);
    CREATE INDEX IX_Rentals_RenterId ON [dbo].[Rentals]([RenterId]);
    CREATE INDEX IX_Rentals_OwnerId ON [dbo].[Rentals]([OwnerId]);
    CREATE INDEX IX_Rentals_Status ON [dbo].[Rentals]([Status]);
    CREATE INDEX IX_Rentals_StartDate ON [dbo].[Rentals]([StartDate]);
    CREATE INDEX IX_Rentals_EndDate ON [dbo].[Rentals]([EndDate]);
    PRINT 'Table Rentals created successfully.';
END
GO

-- =============================================
-- Table: Payments
-- Description: Paiements des réservations (Stripe)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Payments]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Payments] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [RentalId] UNIQUEIDENTIFIER NOT NULL,
        [UserId] UNIQUEIDENTIFIER NOT NULL,
        [Type] INT NOT NULL, -- 0=Rental, 1=Deposit, 2=Refund, 3=LateFee
        [Method] INT NOT NULL, -- 0=CreditCard, 1=DebitCard, 2=Visa, 3=MasterCard, 4=AmericanExpress, 5=PayPal
        [Amount] DECIMAL(18,2) NOT NULL,
        [Status] INT NOT NULL DEFAULT 0, -- 0=Pending, 1=Processing, 2=Succeeded, 3=Failed, 4=Refunded, 5=PartiallyRefunded
        [TransactionId] NVARCHAR(100) NULL,
        [PaymentIntentId] NVARCHAR(200) NULL,
        [StripeChargeId] NVARCHAR(200) NULL,
        [PayPalOrderId] NVARCHAR(200) NULL,
        [ProcessedAt] DATETIME2 NULL,
        [FailureReason] NVARCHAR(500) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        CONSTRAINT FK_Payments_Rentals FOREIGN KEY ([RentalId]) REFERENCES [dbo].[Rentals]([Id]),
        CONSTRAINT FK_Payments_Users FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id])
    );
    
    CREATE INDEX IX_Payments_RentalId ON [dbo].[Payments]([RentalId]);
    CREATE INDEX IX_Payments_UserId ON [dbo].[Payments]([UserId]);
    CREATE INDEX IX_Payments_Status ON [dbo].[Payments]([Status]);
    CREATE INDEX IX_Payments_PaymentIntentId ON [dbo].[Payments]([PaymentIntentId]);
    PRINT 'Table Payments created successfully.';
END
GO

-- =============================================
-- Table: Reviews
-- Description: Avis et notes après location
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Reviews]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Reviews] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [RentalId] UNIQUEIDENTIFIER NOT NULL UNIQUE,
        [ReviewerId] UNIQUEIDENTIFIER NOT NULL,
        [ReviewedUserId] UNIQUEIDENTIFIER NOT NULL,
        [GarmentId] UNIQUEIDENTIFIER NOT NULL,
        [Rating] INT NOT NULL CHECK ([Rating] >= 1 AND [Rating] <= 5),
        [Comment] NVARCHAR(MAX) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        CONSTRAINT FK_Reviews_Rentals FOREIGN KEY ([RentalId]) REFERENCES [dbo].[Rentals]([Id]),
        CONSTRAINT FK_Reviews_Reviewers FOREIGN KEY ([ReviewerId]) REFERENCES [dbo].[Users]([Id]),
        CONSTRAINT FK_Reviews_ReviewedUsers FOREIGN KEY ([ReviewedUserId]) REFERENCES [dbo].[Users]([Id]),
        CONSTRAINT FK_Reviews_Garments FOREIGN KEY ([GarmentId]) REFERENCES [dbo].[Garments]([Id])
    );
    
    CREATE INDEX IX_Reviews_RentalId ON [dbo].[Reviews]([RentalId]);
    CREATE INDEX IX_Reviews_ReviewerId ON [dbo].[Reviews]([ReviewerId]);
    CREATE INDEX IX_Reviews_ReviewedUserId ON [dbo].[Reviews]([ReviewedUserId]);
    CREATE INDEX IX_Reviews_GarmentId ON [dbo].[Reviews]([GarmentId]);
    PRINT 'Table Reviews created successfully.';
END
GO

-- =============================================
-- Table: Conversations
-- Description: Conversations entre utilisateurs concernant un vêtement
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
        CONSTRAINT FK_Conversations_Garments FOREIGN KEY ([GarmentId]) REFERENCES [dbo].[Garments]([Id]),
        CONSTRAINT FK_Conversations_User1 FOREIGN KEY ([User1Id]) REFERENCES [dbo].[Users]([Id]),
        CONSTRAINT FK_Conversations_User2 FOREIGN KEY ([User2Id]) REFERENCES [dbo].[Users]([Id])
    );
    
    CREATE INDEX IX_Conversations_User1Id ON [dbo].[Conversations]([User1Id]);
    CREATE INDEX IX_Conversations_User2Id ON [dbo].[Conversations]([User2Id]);
    CREATE INDEX IX_Conversations_GarmentId ON [dbo].[Conversations]([GarmentId]);
    CREATE INDEX IX_Conversations_LastMessageAt ON [dbo].[Conversations]([LastMessageAt]);
    CREATE UNIQUE INDEX IX_Conversations_GarmentId_Users ON [dbo].[Conversations]([GarmentId], [User1Id], [User2Id]);
    PRINT 'Table Conversations created successfully.';
END
GO

-- =============================================
-- Table: Messages
-- Description: Messages échangés dans les conversations
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
        CONSTRAINT FK_Messages_Senders FOREIGN KEY ([SenderId]) REFERENCES [dbo].[Users]([Id])
    );
    
    CREATE INDEX IX_Messages_ConversationId ON [dbo].[Messages]([ConversationId]);
    CREATE INDEX IX_Messages_SenderId ON [dbo].[Messages]([SenderId]);
    CREATE INDEX IX_Messages_CreatedAt ON [dbo].[Messages]([CreatedAt]);
    CREATE INDEX IX_Messages_IsRead ON [dbo].[Messages]([IsRead]);
    PRINT 'Table Messages created successfully.';
END
GO

-- =============================================
-- Table: GarmentAvailabilities
-- Description: Calendrier de disponibilité des vêtements (3 mois)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GarmentAvailabilities]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[GarmentAvailabilities] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [GarmentId] UNIQUEIDENTIFIER NOT NULL,
        [Date] DATE NOT NULL,
        [IsAvailable] BIT NOT NULL DEFAULT 1,
        [BlockedReason] INT NULL, -- 0=OwnerBlocked, 1=Rental, 2=Maintenance
        [RentalId] UNIQUEIDENTIFIER NULL,
        [Notes] NVARCHAR(500) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NULL,
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        CONSTRAINT FK_GarmentAvailabilities_Garments FOREIGN KEY ([GarmentId]) REFERENCES [dbo].[Garments]([Id]) ON DELETE CASCADE,
        CONSTRAINT FK_GarmentAvailabilities_Rentals FOREIGN KEY ([RentalId]) REFERENCES [dbo].[Rentals]([Id]) ON DELETE SET NULL
    );
    
    CREATE INDEX IX_GarmentAvailabilities_GarmentId ON [dbo].[GarmentAvailabilities]([GarmentId]);
    CREATE INDEX IX_GarmentAvailabilities_Date ON [dbo].[GarmentAvailabilities]([Date]);
    CREATE UNIQUE INDEX IX_GarmentAvailabilities_GarmentId_Date_Unique ON [dbo].[GarmentAvailabilities]([GarmentId], [Date]);
    PRINT 'Table GarmentAvailabilities created successfully.';
END
GO

-- =============================================
-- Vues utiles
-- =============================================

-- Vue: Statistiques des vêtements
IF OBJECT_ID('vw_GarmentStats', 'V') IS NOT NULL
    DROP VIEW vw_GarmentStats;
GO

CREATE VIEW vw_GarmentStats AS
SELECT 
    g.Id,
    g.Title,
    g.OwnerId,
    u.FirstName + ' ' + u.LastName AS OwnerName,
    g.DailyPrice,
    g.City,
    g.ViewCount,
    COUNT(DISTINCT r.Id) AS TotalRentals,
    ISNULL(AVG(CAST(rev.Rating AS DECIMAL(3,2))), 0) AS AverageRating,
    COUNT(DISTINCT rev.Id) AS ReviewCount,
    g.CreatedAt
FROM Garments g
INNER JOIN Users u ON g.OwnerId = u.Id
LEFT JOIN Rentals r ON g.Id = r.GarmentId AND r.Status IN (3, 4) -- Active or Completed
LEFT JOIN Reviews rev ON g.Id = rev.GarmentId
WHERE g.IsDeleted = 0
GROUP BY g.Id, g.Title, g.OwnerId, u.FirstName, u.LastName, g.DailyPrice, g.City, g.ViewCount, g.CreatedAt;
GO

-- Vue: Revenus par propriétaire
IF OBJECT_ID('vw_OwnerRevenue', 'V') IS NOT NULL
    DROP VIEW vw_OwnerRevenue;
GO

CREATE VIEW vw_OwnerRevenue AS
SELECT 
    u.Id AS OwnerId,
    u.FirstName + ' ' + u.LastName AS OwnerName,
    COUNT(DISTINCT g.Id) AS TotalGarments,
    COUNT(DISTINCT r.Id) AS TotalRentals,
    ISNULL(SUM(CASE WHEN r.Status IN (3, 4) THEN r.TotalPrice ELSE 0 END), 0) AS TotalRevenue,
    ISNULL(AVG(CAST(rev.Rating AS DECIMAL(3,2))), 0) AS AverageRating
FROM Users u
LEFT JOIN Garments g ON u.Id = g.OwnerId AND g.IsDeleted = 0
LEFT JOIN Rentals r ON g.Id = r.GarmentId
LEFT JOIN Reviews rev ON u.Id = rev.ReviewedUserId
WHERE u.IsDeleted = 0
GROUP BY u.Id, u.FirstName, u.LastName;
GO

PRINT '==============================================';
PRINT 'Database schema created successfully!';
PRINT 'Tables created: Users, RefreshTokens, Garments, GarmentImages, Rentals, Payments, Reviews, Conversations, Messages, GarmentAvailabilities';
PRINT 'Views created: vw_GarmentStats, vw_OwnerRevenue';
PRINT '==============================================';
GO
