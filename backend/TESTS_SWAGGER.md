# 🧪 Guide de Tests Swagger - SuitForU API

## 📋 Prérequis
- ✅ API lancée sur http://localhost:5156
- ✅ Accéder à http://localhost:5156/swagger
- ✅ Base de données SQL Server "SuitForU" accessible

---

## 🔐 PHASE 1 : Tests d'Authentification

### ✅ Test 1.1 : Inscription (Register)
**Endpoint :** `POST /api/auth/register`

**Cliquer sur l'endpoint > "Try it out"**

**Body JSON :**
```json
{
  "email": "john.doe@test.com",
  "password": "Test123!@#",
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "0612345678"
}
```

**Résultat attendu :** 
- ✅ Status 200 OK
- ✅ Retourne `token` (JWT) et `refreshToken`
- ✅ Message : "Inscription réussie"

**Vérifications :**
- Le `token` commence par `eyJ...`
- Le `refreshToken` est une chaîne de 88 caractères (Base64)
- `userId`, `email`, `firstName`, `lastName` sont retournés

---

### ✅ Test 1.2 : Connexion (Login)
**Endpoint :** `POST /api/auth/login`

**Body JSON :**
```json
{
  "email": "john.doe@test.com",
  "password": "Test123!@#"
}
```

**Résultat attendu :**
- ✅ Status 200 OK
- ✅ Retourne nouveaux `token` et `refreshToken`
- ✅ Message : "Connexion réussie"

**⚠️ Test d'erreur - Mauvais mot de passe :**
```json
{
  "email": "john.doe@test.com",
  "password": "WrongPassword123"
}
```
**Résultat attendu :** Status 401 Unauthorized

---

### ✅ Test 1.3 : Autorisation JWT
**Avant de continuer, copier le `token` du test précédent**

1. **Cliquer sur le bouton "Authorize" 🔒** (en haut à droite de Swagger)
2. Dans le champ "Value", entrer :
   ```
   Bearer eyJhbGc...VotreTokenIci
   ```
   ⚠️ **Important :** Laisser un espace après "Bearer"
3. Cliquer sur "Authorize" puis "Close"

**🟢 L'icône du cadenas devient vert = Authentifié**

---

### ✅ Test 1.4 : Vérifier l'authentification (Me)
**Endpoint :** `GET /api/auth/me`

**Headers automatiques :** `Authorization: Bearer {token}`

**Résultat attendu :**
- ✅ Status 200 OK
- ✅ Retourne `userId` et `email` de l'utilisateur connecté

**⚠️ Test sans token :**
- Cliquer sur "Authorize" > "Logout"
- Réessayer l'endpoint
- **Résultat attendu :** Status 401 Unauthorized

---

### ✅ Test 1.5 : Rafraîchir le token (Refresh)
**Endpoint :** `POST /api/auth/refresh`

**Body JSON :**
```json
{
  "refreshToken": "VotreRefreshTokenDuLogin"
}
```

**Résultat attendu :**
- ✅ Status 200 OK
- ✅ Nouveau `token` et nouveau `refreshToken` générés
- ✅ L'ancien `refreshToken` est révoqué (ne peut plus être réutilisé)

**🔒 Test de sécurité - Réutiliser l'ancien token :**
- Réessayer avec le même `refreshToken`
- **Résultat attendu :** Status 401 Unauthorized
- **Message :** "Token has been revoked" ou "Token reuse detected"

---

### ✅ Test 1.6 : Déconnexion (Logout)
**Endpoint :** `POST /api/auth/logout`

**Body JSON :**
```json
{
  "refreshToken": "VotreRefreshTokenActuel"
}
```

**Résultat attendu :**
- ✅ Status 200 OK
- ✅ Message : "Déconnexion réussie"
- ✅ Le `refreshToken` est révoqué

**Vérification :**
- Réessayer de rafraîchir avec ce token
- **Résultat attendu :** Status 401 Unauthorized

---

### ✅ Test 1.7 : Inscription utilisateur 2 (pour tests propriétaire)
**Créer un deuxième utilisateur pour tester les permissions**

**Body JSON :**
```json
{
  "email": "jane.smith@test.com",
  "password": "Test456!@#",
  "firstName": "Jane",
  "lastName": "Smith",
  "phoneNumber": "0698765432"
}
```

**Résultat attendu :** ✅ Status 200 OK

**💡 Conserver les 2 tokens pour les tests suivants**

---

## 👔 PHASE 2 : Tests Garments (Vêtements)

### ✅ Test 2.1 : Créer un vêtement
**Endpoint :** `POST /api/garments`

**🔐 Authentification requise** (utiliser le token de John Doe)

**Body JSON :**
```json
{
  "name": "Costume Hugo Boss Noir",
  "description": "Costume 3 pièces en laine, parfait pour mariages et événements formels. État neuf.",
  "type": "Suit",
  "size": "L",
  "brand": "Hugo Boss",
  "color": "Noir",
  "pricePerDay": 45.00,
  "securityDeposit": 200.00,
  "city": "Paris",
  "address": "15 Rue de Rivoli, 75001 Paris"
}
```

**Résultat attendu :**
- ✅ Status 201 Created
- ✅ Retourne l'objet `Garment` créé avec un `id`
- ✅ `ownerId` = userId de John Doe
- ✅ `isAvailable` = true
- ✅ `createdAt` = date actuelle

**💡 Copier le `id` du vêtement créé pour les tests suivants**

---

### ✅ Test 2.2 : Créer plusieurs vêtements (pour recherche)
**Créer 3 autres vêtements pour tester la recherche**

**Vêtement 2 :**
```json
{
  "name": "Chemise Ralph Lauren Blanche",
  "description": "Chemise classique en coton, col français",
  "type": "Shirt",
  "size": "M",
  "brand": "Ralph Lauren",
  "color": "Blanc",
  "pricePerDay": 15.00,
  "securityDeposit": 50.00,
  "city": "Paris",
  "address": "20 Avenue des Champs-Élysées, 75008 Paris"
}
```

**Vêtement 3 :**
```json
{
  "name": "Robe Chanel Rouge",
  "description": "Robe de soirée élégante",
  "type": "Dress",
  "size": "S",
  "brand": "Chanel",
  "color": "Rouge",
  "pricePerDay": 80.00,
  "securityDeposit": 500.00,
  "city": "Lyon",
  "address": "10 Place Bellecour, 69002 Lyon"
}
```

**Vêtement 4 :**
```json
{
  "name": "Pantalon Armani Gris",
  "description": "Pantalon de costume ajusté",
  "type": "Pants",
  "size": "L",
  "brand": "Armani",
  "color": "Gris",
  "pricePerDay": 25.00,
  "securityDeposit": 100.00,
  "city": "Paris",
  "address": "5 Rue du Faubourg Saint-Honoré, 75008 Paris"
}
```

---

### ✅ Test 2.3 : Rechercher tous les vêtements (sans filtre)
**Endpoint :** `GET /api/garments`

**Paramètres :** Laisser vides (ou page=1, pageSize=10)

**Résultat attendu :**
- ✅ Status 200 OK
- ✅ Retourne `items` (liste de vêtements)
- ✅ Retourne `totalCount` = 4
- ✅ Retourne `currentPage` = 1, `totalPages` = 1

---

### ✅ Test 2.4 : Rechercher par ville
**Endpoint :** `GET /api/garments`

**Paramètres :**
- `city` = `Paris`
- `page` = `1`
- `pageSize` = `10`

**Résultat attendu :**
- ✅ Status 200 OK
- ✅ Retourne 3 vêtements (costume, chemise, pantalon)
- ✅ Tous ont `city` = "Paris"

---

### ✅ Test 2.5 : Rechercher par prix
**Endpoint :** `GET /api/garments`

**Paramètres :**
- `minPrice` = `20`
- `maxPrice` = `50`

**Résultat attendu :**
- ✅ Status 200 OK
- ✅ Retourne 2 vêtements (costume à 45€, pantalon à 25€)
- ✅ Tous ont `pricePerDay` entre 20 et 50

---

### ✅ Test 2.6 : Rechercher par type
**Endpoint :** `GET /api/garments`

**Paramètres :**
- `type` = `Suit`

**Résultat attendu :**
- ✅ Status 200 OK
- ✅ Retourne 1 vêtement (le costume Hugo Boss)

---

### ✅ Test 2.7 : Rechercher par taille
**Endpoint :** `GET /api/garments`

**Paramètres :**
- `size` = `L`

**Résultat attendu :**
- ✅ Status 200 OK
- ✅ Retourne 2 vêtements (costume L, pantalon L)

---

### ✅ Test 2.8 : Recherche combinée
**Endpoint :** `GET /api/garments`

**Paramètres :**
- `city` = `Paris`
- `minPrice` = `10`
- `maxPrice` = `30`
- `size` = `M`

**Résultat attendu :**
- ✅ Status 200 OK
- ✅ Retourne 1 vêtement (chemise Ralph Lauren)

---

### ✅ Test 2.9 : Pagination
**Endpoint :** `GET /api/garments`

**Paramètres :**
- `page` = `1`
- `pageSize` = `2`

**Résultat attendu :**
- ✅ Status 200 OK
- ✅ `items` contient 2 vêtements
- ✅ `totalCount` = 4
- ✅ `totalPages` = 2
- ✅ `currentPage` = 1
- ✅ `hasNextPage` = true

**Tester page 2 :**
- `page` = `2`, `pageSize` = `2`
- ✅ `items` contient 2 vêtements différents
- ✅ `hasNextPage` = false

---

### ✅ Test 2.10 : Récupérer un vêtement par ID
**Endpoint :** `GET /api/garments/{id}`

**Paramètre :** Utiliser l'ID du costume Hugo Boss

**Résultat attendu :**
- ✅ Status 200 OK
- ✅ Retourne tous les détails du vêtement
- ✅ Inclut la liste `images` (vide pour l'instant)
- ✅ `viewCount` augmente de 1 à chaque appel

**Vérification :** Appeler 3 fois l'endpoint, `viewCount` doit passer à 3

---

### ✅ Test 2.11 : Récupérer mes vêtements
**Endpoint :** `GET /api/garments/my-garments`

**🔐 Authentification requise** (token de John Doe)

**Résultat attendu :**
- ✅ Status 200 OK
- ✅ Retourne les 4 vêtements créés par John Doe
- ✅ Tous ont `ownerId` = userId de John

**Changer de token :**
- Se connecter avec Jane Smith
- Appeler `/api/garments/my-garments`
- ✅ Retourne une liste vide (elle n'a pas de vêtements)

---

### ✅ Test 2.12 : Modifier un vêtement (propriétaire)
**Endpoint :** `PUT /api/garments/{id}`

**🔐 Token de John Doe (propriétaire)**

**Paramètre :** ID du costume Hugo Boss

**Body JSON :**
```json
{
  "name": "Costume Hugo Boss Noir Premium",
  "description": "Costume 3 pièces en laine italienne, nettoyé à sec après chaque location",
  "pricePerDay": 50.00,
  "securityDeposit": 250.00,
  "isAvailable": true
}
```

**Résultat attendu :**
- ✅ Status 200 OK
- ✅ Vêtement mis à jour avec les nouvelles valeurs
- ✅ `name` = "Costume Hugo Boss Noir Premium"
- ✅ `pricePerDay` = 50.00

---

### ✅ Test 2.13 : Modifier un vêtement (non propriétaire) - ERREUR ATTENDUE
**Endpoint :** `PUT /api/garments/{id}`

**🔐 Token de Jane Smith (NON propriétaire)**

**Paramètre :** ID du costume de John

**Body JSON :**
```json
{
  "name": "Je vole le costume",
  "pricePerDay": 1.00
}
```

**Résultat attendu :**
- ✅ Status 403 Forbidden
- ✅ Message d'erreur de permission

---

### ✅ Test 2.14 : Upload d'image (Image 1)
**Endpoint :** `POST /api/garments/{id}/images`

**🔐 Token de John Doe (propriétaire)**

**Paramètre :** ID du costume Hugo Boss

**Body :** 
- Type : `multipart/form-data`
- Champ : `file`
- Fichier : Une image JPEG/PNG de test (< 5MB)

**Résultat attendu :**
- ✅ Status 200 OK
- ✅ Retourne l'URL de l'image uploadée
- ✅ Format : `/uploads/garments/{garmentId}/{filename}`

---

### ✅ Test 2.15 : Upload de 2 autres images
**Répéter le test 2.14 avec 2 autres images**

**Résultat attendu :**
- ✅ 3 images au total uploadées

---

### ✅ Test 2.16 : Upload 4ème image - ERREUR ATTENDUE
**Endpoint :** `POST /api/garments/{id}/images`

**Essayer d'uploader une 4ème image**

**Résultat attendu :**
- ✅ Status 400 Bad Request
- ✅ Message : "Nombre maximum d'images atteint (3)"

---

### ✅ Test 2.17 : Upload fichier non-image - ERREUR ATTENDUE
**Endpoint :** `POST /api/garments/{id}/images`

**Body :** Uploader un fichier PDF ou TXT

**Résultat attendu :**
- ✅ Status 400 Bad Request
- ✅ Message : "Type de fichier non autorisé. Formats acceptés : JPEG, PNG, WebP"

---

### ✅ Test 2.18 : Upload fichier > 5MB - ERREUR ATTENDUE
**Endpoint :** `POST /api/garments/{id}/images`

**Body :** Uploader une image > 5MB

**Résultat attendu :**
- ✅ Status 400 Bad Request
- ✅ Message : "La taille du fichier ne doit pas dépasser 5MB"

---

### ✅ Test 2.19 : Supprimer un vêtement
**Endpoint :** `DELETE /api/garments/{id}`

**🔐 Token de John Doe (propriétaire)**

**Paramètre :** ID de la chemise Ralph Lauren

**Résultat attendu :**
- ✅ Status 200 OK
- ✅ Message : "Vêtement supprimé avec succès"

**Vérification :**
- Appeler `GET /api/garments`
- ✅ La chemise n'apparaît plus dans les résultats (soft delete)

---

### ✅ Test 2.20 : Supprimer un vêtement (non propriétaire) - ERREUR ATTENDUE
**Endpoint :** `DELETE /api/garments/{id}`

**🔐 Token de Jane Smith (NON propriétaire)**

**Paramètre :** ID du pantalon Armani (appartenant à John)

**Résultat attendu :**
- ✅ Status 403 Forbidden

---

## 🔐 PHASE 3 : Tests de Sécurité

### ✅ Test 3.1 : Accès endpoint protégé sans token
**Endpoint :** `POST /api/garments`

**Se déconnecter (Authorize > Logout)**

**Body JSON :**
```json
{
  "name": "Test sans auth",
  "type": "Suit",
  "pricePerDay": 10
}
```

**Résultat attendu :**
- ✅ Status 401 Unauthorized

---

### ✅ Test 3.2 : Token expiré (simuler)
**Attendre que le JWT expire (si expiresIn < 1h dans config)**

**Endpoint :** `GET /api/auth/me`

**Résultat attendu après expiration :**
- ✅ Status 401 Unauthorized
- ✅ Message : "Token has expired"

---

### ✅ Test 3.3 : Token malformé
**Modifier manuellement le token dans "Authorize"**

**Token invalide :**
```
Bearer abc123TokenInvalide
```

**Endpoint :** `GET /api/auth/me`

**Résultat attendu :**
- ✅ Status 401 Unauthorized

---

### ✅ Test 3.4 : Refresh Token Rotation - Détection de réutilisation
**Scénario :**
1. Login > obtenir `refreshToken1`
2. Refresh avec `refreshToken1` > obtenir `refreshToken2`
3. Refresh avec `refreshToken2` > obtenir `refreshToken3`
4. **Réutiliser `refreshToken1`** (déjà révoqué)

**Résultat attendu :**
- ✅ Status 401 Unauthorized
- ✅ Message : "Token reuse detected"
- ✅ **TOUS les refresh tokens de l'utilisateur sont révoqués** (sécurité)

---

## 📊 Récapitulatif des Tests

| Phase | Tests | Durée estimée |
|-------|-------|---------------|
| **1. Authentication** | 7 tests | ~5 minutes |
| **2. Garments** | 20 tests | ~15 minutes |
| **3. Sécurité** | 4 tests | ~3 minutes |
| **TOTAL** | **31 tests** | **~23 minutes** |

---

## ✅ Checklist de Validation

### Authentication
- [ ] Inscription OK
- [ ] Login OK
- [ ] Refresh Token OK
- [ ] Rotation fonctionne (ancien token révoqué)
- [ ] Logout révoque le token
- [ ] Endpoint protégé avec JWT fonctionne

### Garments
- [ ] Création vêtement OK
- [ ] Recherche avec filtres OK
- [ ] Pagination OK
- [ ] Modification par propriétaire OK
- [ ] Modification par non-propriétaire INTERDIT
- [ ] Upload 3 images max OK
- [ ] Upload 4ème image REFUSÉ
- [ ] Fichier non-image REFUSÉ
- [ ] Suppression par propriétaire OK
- [ ] Suppression par non-propriétaire INTERDIT

### Sécurité
- [ ] Endpoint protégé sans token REFUSÉ
- [ ] Token malformé REFUSÉ
- [ ] Réutilisation refresh token DÉTECTÉE

---

## 🚀 Prochaines Étapes

Une fois ces tests validés :
1. **RentalService** - Système de réservation
2. **PaymentService** - Intégration Stripe
3. Tests E2E complets

---

**📝 Note :** Gardez une trace des `userId`, `garmentId`, et tokens pendant les tests pour faciliter les vérifications.
