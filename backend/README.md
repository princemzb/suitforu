# Backend SuitForU - API .NET 9

## 🏗️ Architecture

L'API suit une **Clean Architecture** avec 4 couches :

1. **Domain** : Entités, Enums, Interfaces pures
2. **Application** : DTOs, Interfaces de services, Validators, Mappings
3. **Infrastructure** : DbContext, Repositories, Services externes
4. **API** : Controllers, Middleware, Configuration

## 📋 Prérequis

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/sql-server) ou SQL Server LocalDB
- Visual Studio 2022, VS Code ou JetBrains Rider

## 🚀 Installation

### 1. Cloner le projet

```bash
git clone https://github.com/votre-repo/suitforu.git
cd suitforu/backend
```

### 2. Restaurer les packages

```bash
dotnet restore
```

### 3. Configuration

Modifier `appsettings.Development.json` :

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SuitForUDb;User Id=sa;Password=YourPassword;TrustServerCertificate=true"
  },
  "Jwt": {
    "SecretKey": "CHANGE_THIS_TO_A_SECURE_KEY_AT_LEAST_32_CHARS",
    "Issuer": "SuitForU",
    "Audience": "SuitForU"
  }
}
```

### 4. Créer la base de données

```bash
cd src/SuitForU.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../SuitForU.API
dotnet ef database update --startup-project ../SuitForU.API
```

### 5. Lancer l'API

```bash
cd ../SuitForU.API
dotnet run
```

L'API sera disponible sur : `https://localhost:5001` et `http://localhost:5000`

La documentation Swagger : `https://localhost:5001` (page d'accueil en développement)

## 📂 Structure du Projet

```
backend/
├── src/
│   ├── SuitForU.Domain/
│   │   ├── Common/
│   │   │   └── BaseEntity.cs
│   │   ├── Entities/
│   │   │   ├── User.cs
│   │   │   ├── Garment.cs
│   │   │   ├── GarmentImage.cs
│   │   │   ├── Rental.cs
│   │   │   ├── Payment.cs
│   │   │   ├── Review.cs
│   │   │   └── RefreshToken.cs
│   │   ├── Enums/
│   │   │   ├── AuthProvider.cs
│   │   │   ├── GarmentType.cs
│   │   │   ├── GarmentCondition.cs
│   │   │   ├── RentalStatus.cs
│   │   │   ├── PaymentMethod.cs
│   │   │   └── PaymentStatus.cs
│   │   └── Interfaces/
│   │       ├── IRepository.cs
│   │       ├── IUnitOfWork.cs
│   │       └── [Specific Repositories]
│   │
│   ├── SuitForU.Application/
│   │   ├── DTOs/
│   │   │   ├── Auth/
│   │   │   ├── Garments/
│   │   │   ├── Rentals/
│   │   │   ├── Payments/
│   │   │   └── Common/
│   │   ├── Interfaces/
│   │   │   ├── IAuthService.cs
│   │   │   ├── IGarmentService.cs
│   │   │   ├── IRentalService.cs
│   │   │   └── IPaymentService.cs
│   │   ├── Validators/
│   │   │   ├── Auth/
│   │   │   ├── Garments/
│   │   │   └── Rentals/
│   │   └── Mappings/
│   │       └── MappingProfile.cs
│   │
│   ├── SuitForU.Infrastructure/
│   │   ├── Persistence/
│   │   │   └── ApplicationDbContext.cs
│   │   ├── Repositories/
│   │   │   ├── Repository.cs
│   │   │   ├── UserRepository.cs
│   │   │   ├── GarmentRepository.cs
│   │   │   ├── RentalRepository.cs
│   │   │   ├── PaymentRepository.cs
│   │   │   ├── RefreshTokenRepository.cs
│   │   │   └── UnitOfWork.cs
│   │   └── Services/
│   │       ├── AuthService.cs
│   │       ├── TokenService.cs
│   │       └── FileStorageService.cs
│   │
│   └── SuitForU.API/
│       ├── Controllers/
│       │   ├── AuthController.cs
│       │   ├── GarmentsController.cs
│       │   ├── RentalsController.cs
│       │   └── PaymentsController.cs
│       ├── Program.cs
│       └── appsettings.json
│
└── tests/
    ├── SuitForU.Application.Tests/
    ├── SuitForU.Infrastructure.Tests/
    └── SuitForU.API.Tests/
```

## 🔑 Endpoints API

### Authentication

```
POST /api/auth/register       - Inscription
POST /api/auth/login          - Connexion
POST /api/auth/refresh        - Rafraîchir le token (rotation automatique)
POST /api/auth/logout         - Déconnexion (révoque le refresh token)
POST /api/auth/external       - OAuth (Google, Facebook, Instagram)
POST /api/auth/confirm-email  - Confirmer l'email
GET  /api/auth/me             - Informations utilisateur connecté
```

### Garments

```
GET    /api/garments              - Liste des vêtements
GET    /api/garments/{id}         - Détails d'un vêtement
POST   /api/garments              - Créer un vêtement
PUT    /api/garments/{id}         - Modifier un vêtement
DELETE /api/garments/{id}         - Supprimer un vêtement
POST   /api/garments/{id}/images  - Ajouter une image
GET    /api/garments/search       - Recherche avancée
GET    /api/garments/my-garments  - Mes vêtements
```

### Rentals

```
GET    /api/rentals              - Liste des locations
GET    /api/rentals/{id}         - Détails d'une location
POST   /api/rentals              - Créer une demande de location
PUT    /api/rentals/{id}/accept  - Accepter une location (propriétaire)
PUT    /api/rentals/{id}/confirm - Confirmer une location (locataire)
PUT    /api/rentals/{id}/extend  - Prolonger une location
PUT    /api/rentals/{id}/cancel  - Annuler une location
GET    /api/rentals/as-renter    - Mes locations en tant que locataire
GET    /api/rentals/as-owner     - Mes locations en tant que propriétaire
```

### Payments

```
POST   /api/payments/create-intent  - Créer un PaymentIntent Stripe
POST   /api/payments/process        - Traiter un paiement
GET    /api/payments/my-payments    - Mes paiements
POST   /api/payments/{id}/refund    - Rembourser un paiement
```

## 🔐 Authentification

L'API utilise JWT Bearer tokens avec **Refresh Token Rotation** pour une sécurité maximale.

### Architecture des Tokens

**Access Token (JWT)**
- Durée de vie : 15 minutes
- Stocké en mémoire côté client
- Contient : `userId`, `email`, `iat`, `exp`
- Utilisé dans le header `Authorization: Bearer {access_token}`

**Refresh Token**
- Durée de vie : 7 jours
- Token cryptographique aléatoire (64 bytes)
- Stocké en base de données avec traçabilité (IP, dates, révocation)
- **Rotation automatique** : Chaque utilisation génère un nouveau token
- **One-time use** : Un token utilisé est révoqué immédiatement

### Flux d'Authentification

```
1. Login → Access Token (15min) + Refresh Token (7 jours)
2. Access Token expire → POST /api/auth/refresh avec Refresh Token
3. Nouveau Access Token + Nouveau Refresh Token (ancien révoqué)
4. Logout → POST /api/auth/logout (révoque le Refresh Token)
```

### Sécurité des Refresh Tokens

✅ **Token Reuse Detection** : Si un token révoqué est réutilisé, tous les tokens de l'utilisateur sont révoqués  
✅ **IP Tracking** : Chaque token stocke l'IP de création et révocation  
✅ **Audit Trail** : Historique complet des connexions  
✅ **Cleanup automatique** : Tokens expirés supprimés après 30 jours  

### Headers pour Endpoints Protégés

```http
Authorization: Bearer {access_token}
```

### Exemple d'utilisation

```bash
# 1. Login
curl -X POST https://api.suitforu.com/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"user@example.com","password":"password123"}'

# Réponse : { "accessToken": "...", "refreshToken": "...", "user": {...} }

# 2. Appel API protégé
curl https://api.suitforu.com/api/garments \
  -H "Authorization: Bearer {accessToken}"

# 3. Refresh quand access token expire
curl -X POST https://api.suitforu.com/api/auth/refresh \
  -H "Content-Type: application/json" \
  -d '{"refreshToken":"..."}'

# 4. Logout
curl -X POST https://api.suitforu.com/api/auth/logout \
  -H "Content-Type: application/json" \
  -d '{"refreshToken":"..."}'
```

## 🧪 Tests

### Exécuter tous les tests

```bash
dotnet test
```

### Exécuter les tests d'un projet spécifique

```bash
dotnet test tests/SuitForU.Application.Tests/SuitForU.Application.Tests.csproj
```

### Coverage

```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

## 🗄️ Migrations Entity Framework

### Créer une migration

```bash
cd src/SuitForU.Infrastructure
dotnet ef migrations add NomDeLaMigration --startup-project ../SuitForU.API
```

### Appliquer les migrations

```bash
dotnet ef database update --startup-project ../SuitForU.API
```

### Supprimer la dernière migration

```bash
dotnet ef migrations remove --startup-project ../SuitForU.API
```

## 📦 Packages NuGet Principaux

- `Microsoft.EntityFrameworkCore` (9.0.x) - ORM
- `Microsoft.EntityFrameworkCore.SqlServer` (9.0.x) - Provider SQL Server
- `Microsoft.AspNetCore.Authentication.JwtBearer` (9.0.x) - JWT Auth
- `AutoMapper` (15.1.0) - Mapping DTOs
- `FluentValidation` (12.1.0) - Validation
- `BCrypt.Net-Next` (4.0.3) - Hash des mots de passe
- `Swashbuckle.AspNetCore` (10.0.1) - Swagger/OpenAPI

## 🚀 Déploiement

### Azure

```bash
# Créer une App Service
az webapp create --resource-group myResourceGroup --plan myAppServicePlan --name suitforu-api

# Déployer
dotnet publish -c Release
az webapp deployment source config-zip --resource-group myResourceGroup --name suitforu-api --src ./publish.zip
```

### Docker

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["SuitForU.API/SuitForU.API.csproj", "SuitForU.API/"]
RUN dotnet restore "SuitForU.API/SuitForU.API.csproj"
COPY . .
WORKDIR "/src/SuitForU.API"
RUN dotnet build "SuitForU.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SuitForU.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SuitForU.API.dll"]
```

## 📖 Documentation Supplémentaire

- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Entity Framework Core](https://learn.microsoft.com/ef/core/)
- [JWT Authentication](https://jwt.io/)
- [AutoMapper](https://automapper.org/)
- [FluentValidation](https://docs.fluentvalidation.net/)

## 📄 Licence

Ce projet est propriétaire. Tous droits réservés.

---

**Développé avec ❤️ pour SuitForU**
