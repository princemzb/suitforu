# 🚀 Guide de Démarrage Rapide - SuitForU

## ⚡ Mise en Route en 5 Minutes

### Prérequis
- [ ] .NET 9 SDK installé
- [ ] SQL Server ou SQL Server LocalDB
- [ ] Visual Studio Code, Visual Studio 2022, ou Rider
- [ ] Git

### Étape 1 : Cloner et Restaurer

```bash
# Cloner le projet
git clone <votre-repo>
cd suitforu

# Restaurer les packages NuGet
cd backend
dotnet restore
```

### Étape 2 : Configurer la Base de Données

```bash
# Option A : Utiliser LocalDB (par défaut)
# La connection string est déjà configurée dans appsettings.Development.json

# Option B : Utiliser SQL Server
# Modifier backend/src/SuitForU.API/appsettings.Development.json :
# "DefaultConnection": "Server=localhost;Database=SuitForUDb;User Id=sa;Password=VotrePassword;TrustServerCertificate=true"

# Créer la base de données
cd src/SuitForU.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../SuitForU.API
dotnet ef database update --startup-project ../SuitForU.API
```

### Étape 3 : Lancer l'API

```bash
cd ../SuitForU.API
dotnet run
```

🎉 **L'API est maintenant disponible sur :**
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`
- Swagger UI: `https://localhost:5001` (page d'accueil)

### Étape 4 : Tester avec Swagger

1. Ouvrir votre navigateur sur `https://localhost:5001`
2. Vous verrez la documentation Swagger/OpenAPI
3. Tester l'endpoint `/api/auth/register` pour créer un utilisateur
4. Copier le token JWT reçu
5. Cliquer sur "Authorize" en haut à droite
6. Saisir : `Bearer {votre-token}`
7. Tester les endpoints protégés

## 📱 Prochaines Étapes

### Pour le Backend

1. **Compléter AuthService**
   ```bash
   # Créer le fichier
   backend/src/SuitForU.Application/Services/AuthService.cs
   ```

2. **Ajouter les autres controllers**
   - GarmentsController
   - RentalsController
   - PaymentsController

3. **Écrire les tests**
   ```bash
   cd backend/tests/SuitForU.Application.Tests
   dotnet test
   ```

### Pour Flutter (À faire)

```bash
# Créer le projet
flutter create --org com.suitforu mobile
cd mobile

# Ajouter les dépendances dans pubspec.yaml
flutter pub add flutter_bloc dio shared_preferences flutter_secure_storage

# Lancer
flutter run
```

### Pour Angular (À faire)

```bash
# Créer le projet
ng new web --routing --style=scss
cd web

# Ajouter Angular Material
ng add @angular/material

# Lancer
ng serve
```

## 🔑 Commandes Utiles

### Backend

```bash
# Build
dotnet build

# Tests
dotnet test

# Tests avec coverage
dotnet test /p:CollectCoverage=true

# Nouvelle migration
dotnet ef migrations add NomMigration -p src/SuitForU.Infrastructure -s src/SuitForU.API

# Appliquer les migrations
dotnet ef database update -p src/SuitForU.Infrastructure -s src/SuitForU.API

# Supprimer la dernière migration
dotnet ef migrations remove -p src/SuitForU.Infrastructure -s src/SuitForU.API

# Créer un script SQL de migration
dotnet ef migrations script -p src/SuitForU.Infrastructure -s src/SuitForU.API

# Publier en Release
dotnet publish -c Release -o ./publish
```

### Flutter

```bash
# Installer les dépendances
flutter pub get

# Générer le code (build_runner)
flutter pub run build_runner build --delete-conflicting-outputs

# Lancer sur un émulateur/device
flutter run

# Build APK Android
flutter build apk --release

# Build iOS
flutter build ios --release

# Tests
flutter test

# Analyser le code
flutter analyze

# Nettoyer
flutter clean
```

### Angular

```bash
# Installer les dépendances
npm install

# Lancer le serveur de dev
ng serve

# Build production
ng build --configuration=production

# Tests unitaires
ng test

# Tests e2e
ng e2e

# Analyser le bundle
ng build --stats-json
npx webpack-bundle-analyzer dist/web/stats.json

# Générer un composant
ng generate component features/auth/login

# Générer un service
ng generate service core/services/auth
```

## 🐛 Résolution de Problèmes

### Erreur : "Unable to connect to database"
```bash
# Vérifier la connection string
# Vérifier que SQL Server est démarré
# Sur Windows : Services > SQL Server

# Tester la connexion
sqlcmd -S (localdb)\mssqllocaldb -Q "SELECT @@VERSION"
```

### Erreur : "JWT Secret Key not configured"
```bash
# S'assurer que appsettings.Development.json existe
# Et contient une clé JWT valide (min 32 caractères)
```

### Erreur de migration EF Core
```bash
# Supprimer la base de données
dotnet ef database drop -p src/SuitForU.Infrastructure -s src/SuitForU.API

# Supprimer toutes les migrations
rm -rf src/SuitForU.Infrastructure/Migrations

# Recréer tout
dotnet ef migrations add InitialCreate -p src/SuitForU.Infrastructure -s src/SuitForU.API
dotnet ef database update -p src/SuitForU.Infrastructure -s src/SuitForU.API
```

### Port déjà utilisé
```bash
# Changer le port dans backend/src/SuitForU.API/Properties/launchSettings.json
# Ou tuer le processus sur le port 5001
# Windows:
netstat -ano | findstr :5001
taskkill /PID <PID> /F
```

## 📚 Ressources

### Documentation Officielle
- [.NET 9](https://learn.microsoft.com/dotnet/core/whats-new/dotnet-9)
- [Entity Framework Core](https://learn.microsoft.com/ef/core/)
- [Flutter](https://flutter.dev/docs)
- [Angular 18](https://angular.io/docs)

### Tutoriels
- [Clean Architecture .NET](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Flutter BLoC Pattern](https://bloclibrary.dev/)
- [Angular Best Practices](https://angular.io/guide/styleguide)

### Outils
- [Postman](https://www.postman.com/) - Test d'API
- [DB Browser for SQLite](https://sqlitebrowser.org/) - Visualiser la DB
- [ngrok](https://ngrok.com/) - Exposer localhost
- [Stripe CLI](https://stripe.com/docs/stripe-cli) - Tester les webhooks

## 💡 Astuces

1. **Utiliser Swagger pour tous les tests d'API** - Plus rapide que Postman pour débuter
2. **Hot Reload .NET** : `dotnet watch run` pour le rechargement automatique
3. **Flutter DevTools** : Super pour déboguer les performances
4. **Angular DevTools** : Extension Chrome pour déboguer Angular
5. **Git Hooks** : Ajouter des hooks pre-commit pour lancer les tests

## 🎯 Checklist de Développement

### Backend
- [ ] Migrations créées et appliquées
- [ ] AuthService implémenté
- [ ] GarmentService implémenté
- [ ] RentalService implémenté
- [ ] PaymentService implémenté
- [ ] Tous les controllers créés
- [ ] Tests unitaires >80%
- [ ] Swagger documenté
- [ ] Middleware d'erreurs
- [ ] Logging configuré

### Flutter
- [ ] Projet créé
- [ ] Architecture BLoC en place
- [ ] Authentification fonctionnelle
- [ ] Liste des vêtements
- [ ] Upload d'images
- [ ] Intégration Stripe
- [ ] i18n FR/EN
- [ ] Tests

### Angular
- [ ] Projet créé
- [ ] Routing configuré
- [ ] Authentification + Guards
- [ ] Module Garments
- [ ] Module Rentals
- [ ] Responsive design
- [ ] i18n
- [ ] Tests

## 📞 Support

En cas de problème :
1. Consulter les logs de l'API
2. Vérifier la configuration (appsettings.json)
3. Consulter IMPLEMENTATION.md pour le statut détaillé
4. Créer une issue sur GitHub

---

**Bon développement ! 🚀**
