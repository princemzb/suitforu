# Base de Données SuitForU - Documentation

## 📊 Vue d'ensemble

**SGBD:** SQL Server  
**Nom:** SuitForU  
**Total tables:** 10  
**Migrations appliquées:** 3

---

## 🗂️ Schéma de la base de données

### Tables principales

#### 1. **Users** 
Utilisateurs de la plateforme (locataires et propriétaires)

| Colonne | Type | Description |
|---------|------|-------------|
| Id | UNIQUEIDENTIFIER | PK, clé primaire |
| Email | NVARCHAR(256) | Email unique, obligatoire |
| PasswordHash | NVARCHAR(MAX) | Hash BCrypt du mot de passe |
| FirstName | NVARCHAR(100) | Prénom |
| LastName | NVARCHAR(100) | Nom |
| PhoneNumber | NVARCHAR(20) | Téléphone |
| Address | NVARCHAR(500) | Adresse complète |
| City | NVARCHAR(100) | Ville |
| PostalCode | NVARCHAR(20) | Code postal |
| Country | NVARCHAR(100) | Pays (default: France) |
| ProfilePictureUrl | NVARCHAR(500) | URL photo de profil |
| Bio | NVARCHAR(MAX) | Description |
| IsEmailConfirmed | BIT | Email confirmé (default: 0) |
| EmailConfirmationToken | NVARCHAR(MAX) | Token de confirmation |
| AuthProvider | NVARCHAR(50) | Fournisseur auth externe |
| ExternalAuthId | NVARCHAR(256) | ID auth externe |
| Rating | DECIMAL(3,2) | Note moyenne (0-5) |
| RatingCount | INT | Nombre d'avis |
| CreatedAt | DATETIME2 | Date de création |
| UpdatedAt | DATETIME2 | Date de mise à jour |
| IsDeleted | BIT | Soft delete (default: 0) |

**Index:**
- `IX_Users_Email` sur Email
- `IX_Users_CreatedAt` sur CreatedAt

---

#### 2. **RefreshTokens**
Tokens de rafraîchissement JWT avec rotation

| Colonne | Type | Description |
|---------|------|-------------|
| Id | UNIQUEIDENTIFIER | PK |
| Token | NVARCHAR(500) | Token cryptographique unique |
| UserId | UNIQUEIDENTIFIER | FK vers Users |
| ExpiresAt | DATETIME2 | Date d'expiration |
| IsRevoked | BIT | Token révoqué (default: 0) |
| RevokedAt | DATETIME2 | Date de révocation |
| CreatedByIp | NVARCHAR(50) | IP de création |
| RevokedByIp | NVARCHAR(50) | IP de révocation |
| ReplacedByToken | NVARCHAR(500) | Token de remplacement |
| CreatedAt | DATETIME2 | Date de création |
| UpdatedAt | DATETIME2 | Date de mise à jour |

**Relations:**
- `FK_RefreshTokens_Users`: UserId → Users.Id (CASCADE)

**Index:**
- `IX_RefreshTokens_Token` sur Token (UNIQUE)
- `IX_RefreshTokens_UserId` sur UserId
- `IX_RefreshTokens_ExpiresAt` sur ExpiresAt

---

#### 3. **Garments**
Vêtements de cérémonie disponibles à la location

| Colonne | Type | Description |
|---------|------|-------------|
| Id | UNIQUEIDENTIFIER | PK |
| OwnerId | UNIQUEIDENTIFIER | FK vers Users (propriétaire) |
| Title | NVARCHAR(200) | Titre du vêtement |
| Description | NVARCHAR(MAX) | Description détaillée |
| Type | INT | Enum: 0=Suit, 1=Dress, 2=Tuxedo, etc. |
| Condition | INT | Enum: 0=New, 1=LikeNew, 2=Good, 3=Fair |
| Size | NVARCHAR(10) | Taille (S, M, L, XL, etc.) |
| Brand | NVARCHAR(100) | Marque |
| Color | NVARCHAR(50) | Couleur |
| DailyPrice | DECIMAL(18,2) | Prix par jour |
| DepositAmount | DECIMAL(18,2) | Montant de la caution |
| PickupAddress | NVARCHAR(500) | Adresse de récupération |
| City | NVARCHAR(100) | Ville |
| PostalCode | NVARCHAR(20) | Code postal |
| Country | NVARCHAR(100) | Pays (default: France) |
| IsAvailable | BIT | Disponible (default: 1) |
| ViewCount | INT | Nombre de vues (default: 0) |
| CreatedAt | DATETIME2 | Date de création |
| UpdatedAt | DATETIME2 | Date de mise à jour |
| IsDeleted | BIT | Soft delete (default: 0) |

**Relations:**
- `FK_Garments_Users`: OwnerId → Users.Id

**Index:**
- `IX_Garments_OwnerId` sur OwnerId
- `IX_Garments_City` sur City
- `IX_Garments_Type` sur Type
- `IX_Garments_DailyPrice` sur DailyPrice
- `IX_Garments_IsAvailable` sur IsAvailable
- `IX_Garments_CreatedAt` sur CreatedAt

---

#### 4. **GarmentImages**
Images des vêtements (max 3 par vêtement)

| Colonne | Type | Description |
|---------|------|-------------|
| Id | UNIQUEIDENTIFIER | PK |
| GarmentId | UNIQUEIDENTIFIER | FK vers Garments |
| ImageUrl | NVARCHAR(500) | URL de l'image |
| IsPrimary | BIT | Image principale (default: 0) |
| DisplayOrder | INT | Ordre d'affichage (default: 0) |
| CreatedAt | DATETIME2 | Date de création |
| UpdatedAt | DATETIME2 | Date de mise à jour |
| IsDeleted | BIT | Soft delete (default: 0) |

**Relations:**
- `FK_GarmentImages_Garments`: GarmentId → Garments.Id (CASCADE)

**Index:**
- `IX_GarmentImages_GarmentId` sur GarmentId

---

#### 5. **Rentals**
Réservations de vêtements avec workflow complet

| Colonne | Type | Description |
|---------|------|-------------|
| Id | UNIQUEIDENTIFIER | PK |
| GarmentId | UNIQUEIDENTIFIER | FK vers Garments |
| RenterId | UNIQUEIDENTIFIER | FK vers Users (locataire) |
| OwnerId | UNIQUEIDENTIFIER | FK vers Users (propriétaire) |
| StartDate | DATETIME2 | Date de début |
| EndDate | DATETIME2 | Date de fin |
| DurationDays | INT | Durée en jours |
| DailyPrice | DECIMAL(18,2) | Prix journalier |
| TotalPrice | DECIMAL(18,2) | Prix total |
| DepositAmount | DECIMAL(18,2) | Montant de la caution |
| Status | INT | Enum: 0=Pending, 1=OwnerAccepted, 2=Confirmed, 3=Active, 4=Completed, 5=Cancelled |
| OwnerAcceptedAt | DATETIME2 | Date d'acceptation propriétaire |
| RenterConfirmedAt | DATETIME2 | Date de confirmation locataire |
| PickupConfirmedAt | DATETIME2 | Date de récupération |
| ReturnConfirmedAt | DATETIME2 | Date de retour |
| CancellationReason | NVARCHAR(500) | Raison d'annulation |
| CancelledAt | DATETIME2 | Date d'annulation |
| CreatedAt | DATETIME2 | Date de création |
| UpdatedAt | DATETIME2 | Date de mise à jour |
| IsDeleted | BIT | Soft delete (default: 0) |

**Relations:**
- `FK_Rentals_Garments`: GarmentId → Garments.Id
- `FK_Rentals_Renters`: RenterId → Users.Id
- `FK_Rentals_Owners`: OwnerId → Users.Id

**Index:**
- `IX_Rentals_GarmentId` sur GarmentId
- `IX_Rentals_RenterId` sur RenterId
- `IX_Rentals_OwnerId` sur OwnerId
- `IX_Rentals_Status` sur Status
- `IX_Rentals_StartDate` sur StartDate
- `IX_Rentals_EndDate` sur EndDate

---

#### 6. **Payments**
Paiements des réservations (Stripe simulation)

| Colonne | Type | Description |
|---------|------|-------------|
| Id | UNIQUEIDENTIFIER | PK |
| RentalId | UNIQUEIDENTIFIER | FK vers Rentals |
| UserId | UNIQUEIDENTIFIER | FK vers Users |
| Type | INT | Enum: 0=Rental, 1=Deposit, 2=Refund, 3=LateFee |
| Method | INT | Enum: 0=CreditCard, 1=DebitCard, etc. |
| Amount | DECIMAL(18,2) | Montant |
| Status | INT | Enum: 0=Pending, 2=Succeeded, 3=Failed, 4=Refunded |
| TransactionId | NVARCHAR(100) | ID transaction |
| PaymentIntentId | NVARCHAR(200) | Stripe PaymentIntent ID |
| StripeChargeId | NVARCHAR(200) | Stripe Charge ID |
| ProcessedAt | DATETIME2 | Date de traitement |
| FailureReason | NVARCHAR(500) | Raison d'échec |
| CreatedAt | DATETIME2 | Date de création |
| UpdatedAt | DATETIME2 | Date de mise à jour |
| IsDeleted | BIT | Soft delete (default: 0) |

**Relations:**
- `FK_Payments_Rentals`: RentalId → Rentals.Id
- `FK_Payments_Users`: UserId → Users.Id

**Index:**
- `IX_Payments_RentalId` sur RentalId
- `IX_Payments_UserId` sur UserId
- `IX_Payments_Status` sur Status
- `IX_Payments_PaymentIntentId` sur PaymentIntentId

---

#### 7. **Reviews**
Avis et notes après location

| Colonne | Type | Description |
|---------|------|-------------|
| Id | UNIQUEIDENTIFIER | PK |
| RentalId | UNIQUEIDENTIFIER | FK vers Rentals (UNIQUE) |
| ReviewerId | UNIQUEIDENTIFIER | FK vers Users (auteur) |
| ReviewedUserId | UNIQUEIDENTIFIER | FK vers Users (destinataire) |
| GarmentId | UNIQUEIDENTIFIER | FK vers Garments |
| Rating | INT | Note 1-5 |
| Comment | NVARCHAR(MAX) | Commentaire |
| CreatedAt | DATETIME2 | Date de création |
| UpdatedAt | DATETIME2 | Date de mise à jour |
| IsDeleted | BIT | Soft delete (default: 0) |

**Relations:**
- `FK_Reviews_Rentals`: RentalId → Rentals.Id
- `FK_Reviews_Reviewers`: ReviewerId → Users.Id
- `FK_Reviews_ReviewedUsers`: ReviewedUserId → Users.Id
- `FK_Reviews_Garments`: GarmentId → Garments.Id

**Index:**
- `IX_Reviews_RentalId` sur RentalId (UNIQUE)
- `IX_Reviews_ReviewerId` sur ReviewerId
- `IX_Reviews_ReviewedUserId` sur ReviewedUserId
- `IX_Reviews_GarmentId` sur GarmentId

---

#### 8. **Conversations**
Conversations entre utilisateurs autour d'un vêtement

| Colonne | Type | Description |
|---------|------|-------------|
| Id | UNIQUEIDENTIFIER | PK |
| GarmentId | UNIQUEIDENTIFIER | FK vers Garments |
| User1Id | UNIQUEIDENTIFIER | FK vers Users (participant 1) |
| User2Id | UNIQUEIDENTIFIER | FK vers Users (participant 2) |
| LastMessageAt | DATETIME2 | Date du dernier message |
| LastMessageContent | NVARCHAR(500) | Contenu du dernier message |
| LastMessageSenderId | UNIQUEIDENTIFIER | Auteur du dernier message |
| CreatedAt | DATETIME2 | Date de création |
| UpdatedAt | DATETIME2 | Date de mise à jour |
| IsDeleted | BIT | Soft delete (default: 0) |

**Relations:**
- `FK_Conversations_Garments`: GarmentId → Garments.Id
- `FK_Conversations_User1`: User1Id → Users.Id
- `FK_Conversations_User2`: User2Id → Users.Id

**Index:**
- `IX_Conversations_User1Id` sur User1Id
- `IX_Conversations_User2Id` sur User2Id
- `IX_Conversations_GarmentId` sur GarmentId
- `IX_Conversations_LastMessageAt` sur LastMessageAt
- `IX_Conversations_GarmentId_Users` sur (GarmentId, User1Id, User2Id) **UNIQUE**

**Règle métier:** Une seule conversation possible entre 2 users pour un vêtement donné

---

#### 9. **Messages**
Messages échangés dans les conversations

| Colonne | Type | Description |
|---------|------|-------------|
| Id | UNIQUEIDENTIFIER | PK |
| ConversationId | UNIQUEIDENTIFIER | FK vers Conversations |
| SenderId | UNIQUEIDENTIFIER | FK vers Users (expéditeur) |
| Content | NVARCHAR(2000) | Contenu du message |
| IsRead | BIT | Message lu (default: 0) |
| ReadAt | DATETIME2 | Date de lecture |
| CreatedAt | DATETIME2 | Date de création |
| UpdatedAt | DATETIME2 | Date de mise à jour |
| IsDeleted | BIT | Soft delete (default: 0) |

**Relations:**
- `FK_Messages_Conversations`: ConversationId → Conversations.Id (CASCADE)
- `FK_Messages_Senders`: SenderId → Users.Id

**Index:**
- `IX_Messages_ConversationId` sur ConversationId
- `IX_Messages_SenderId` sur SenderId
- `IX_Messages_CreatedAt` sur CreatedAt
- `IX_Messages_IsRead` sur IsRead

---

#### 10. **GarmentAvailabilities**
Calendrier de disponibilité des vêtements (3 mois)

| Colonne | Type | Description |
|---------|------|-------------|
| Id | UNIQUEIDENTIFIER | PK |
| GarmentId | UNIQUEIDENTIFIER | FK vers Garments |
| Date | DATE | Date concernée (jour uniquement) |
| IsAvailable | BIT | Disponible (default: 1) |
| BlockedReason | INT | Enum: 0=OwnerBlocked, 1=Rental, 2=Maintenance |
| RentalId | UNIQUEIDENTIFIER | FK vers Rentals (si bloqué auto) |
| Notes | NVARCHAR(500) | Notes du propriétaire |
| CreatedAt | DATETIME2 | Date de création |
| UpdatedAt | DATETIME2 | Date de mise à jour |
| IsDeleted | BIT | Soft delete (default: 0) |

**Relations:**
- `FK_GarmentAvailabilities_Garments`: GarmentId → Garments.Id (CASCADE)
- `FK_GarmentAvailabilities_Rentals`: RentalId → Rentals.Id (SET NULL)

**Index:**
- `IX_GarmentAvailabilities_GarmentId` sur GarmentId
- `IX_GarmentAvailabilities_Date` sur Date
- `IX_GarmentAvailabilities_GarmentId_Date_Unique` sur (GarmentId, Date) **UNIQUE**

**Règle métier:** 
- Par défaut, toutes les dates sont disponibles
- Le propriétaire peut bloquer/débloquer manuellement
- Les dates sont bloquées automatiquement lors de la confirmation d'une location
- Les dates sont libérées automatiquement lors de l'annulation

---

## 🔄 Migrations

### Migration 1: InitialCreate
- Tables: Users, Garments, GarmentImages, Rentals, Payments, Reviews
- Date: Création initiale du projet

### Migration 2: AddRefreshTokenTable
- Table: RefreshTokens
- Date: Ajout du système de Refresh Token Rotation
- Fonctionnalités: Rotation sécurisée, traçabilité IP, révocation

### Migration 3: AddMessagingSystem
- Tables: Conversations, Messages
- Date: 2025-12-11
- Fonctionnalités: Chat contextuel par vêtement, statut lu/non-lu

### Migration 4: AddAvailabilitySystem
- Table: GarmentAvailabilities
- Date: 2025-12-11
- Fonctionnalités: Calendrier 3 mois, blocage manuel/automatique

---

## 📈 Vues SQL

### vw_GarmentStats
Statistiques des vêtements avec notes et locations

```sql
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
LEFT JOIN Rentals r ON g.Id = r.GarmentId AND r.Status IN (3, 4)
LEFT JOIN Reviews rev ON g.Id = rev.GarmentId
WHERE g.IsDeleted = 0
GROUP BY g.Id, g.Title, g.OwnerId, u.FirstName, u.LastName, g.DailyPrice, g.City, g.ViewCount, g.CreatedAt
```

### vw_OwnerRevenue
Revenus par propriétaire

```sql
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
GROUP BY u.Id, u.FirstName, u.LastName
```

---

## 🔒 Stratégies de suppression

| Table | Stratégie | Description |
|-------|-----------|-------------|
| Users | Soft Delete | `IsDeleted = 1` |
| Garments | Soft Delete | `IsDeleted = 1` |
| Rentals | Soft Delete | `IsDeleted = 1` |
| Payments | Soft Delete | `IsDeleted = 1` |
| Reviews | Soft Delete | `IsDeleted = 1` |
| RefreshTokens | Hard Delete | Révocation avec `IsRevoked = 1` |
| Conversations | Soft Delete | `IsDeleted = 1` |
| Messages | Cascade Delete | Supprimés avec la conversation |
| GarmentImages | Cascade Delete | Supprimés avec le vêtement |
| GarmentAvailabilities | Cascade Delete | Supprimés avec le vêtement |

---

## 📝 Scripts de création

### Script complet
`backend/database_creation_script.sql` - Script idempotent pour créer toute la base

### Scripts de migration
- `backend/migrations/20251211_AddMessagingSystem.sql`
- `backend/migrations/20251211_AddAvailabilitySystem.sql`

---

## 🧪 Commandes utiles

### Créer une migration
```bash
cd backend/src/SuitForU.Infrastructure
dotnet ef migrations add NomDeLaMigration --startup-project ../SuitForU.API
```

### Appliquer les migrations
```bash
dotnet ef database update --startup-project ../SuitForU.API
```

### Générer un script SQL
```bash
dotnet ef migrations script --idempotent --output migration.sql --startup-project ../SuitForU.API
```

### Rollback migration
```bash
dotnet ef database update PreviousMigrationName --startup-project ../SuitForU.API
```
