# API Endpoints Documentation - SuitForU

## 📋 Vue d'ensemble

L'API SuitForU comprend **37 endpoints** répartis sur 6 controllers :
- **Auth** : 7 endpoints (authentification)
- **Garments** : 7 endpoints (vêtements)
- **Rentals** : 7 endpoints (locations)
- **Payments** : 5 endpoints (paiements)
- **Conversations** : 6 endpoints (messagerie)
- **Availability** : 4 endpoints (disponibilité)

---

## 🔐 Authentication (AuthController)

### 1. POST /api/auth/register
Inscription d'un nouvel utilisateur

**Body:**
```json
{
  "email": "user@example.com",
  "password": "SecurePass123!",
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+33612345678",
  "address": "123 rue de la Paix",
  "city": "Paris",
  "postalCode": "75001",
  "country": "France"
}
```

**Response 200:**
```json
{
  "accessToken": "eyJhbGc...",
  "refreshToken": "crypto-secure-token",
  "user": { ... }
}
```

---

### 2. POST /api/auth/login
Connexion utilisateur

**Body:**
```json
{
  "email": "user@example.com",
  "password": "SecurePass123!"
}
```

**Response 200:**
```json
{
  "accessToken": "eyJhbGc...",
  "refreshToken": "crypto-secure-token",
  "user": {
    "id": "guid",
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe"
  }
}
```

---

### 3. POST /api/auth/refresh
Rafraîchir le token d'accès (Refresh Token Rotation)

**Body:**
```json
{
  "refreshToken": "current-refresh-token"
}
```

**Response 200:**
```json
{
  "accessToken": "new-jwt-token",
  "refreshToken": "new-refresh-token"
}
```

---

### 4. POST /api/auth/logout
Déconnexion et révocation du refresh token

**Headers:** `Authorization: Bearer {token}`

**Response 204:** No Content

---

### 5. POST /api/auth/external
Authentification externe (Google, Facebook)

**Body:**
```json
{
  "provider": "Google",
  "externalId": "google-user-id",
  "email": "user@gmail.com",
  "firstName": "John",
  "lastName": "Doe"
}
```

---

### 6. POST /api/auth/confirm-email
Confirmation d'email

**Body:**
```json
{
  "token": "confirmation-token-from-email"
}
```

---

### 7. GET /api/auth/me
Récupérer les informations de l'utilisateur connecté

**Headers:** `Authorization: Bearer {token}`

**Response 200:**
```json
{
  "id": "guid",
  "email": "user@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+33612345678",
  "rating": 4.5,
  "ratingCount": 10
}
```

---

## 👔 Garments (GarmentsController)

### 1. POST /api/garments
Créer un nouveau vêtement

**Headers:** `Authorization: Bearer {token}`

**Body:**
```json
{
  "title": "Costume 3 pièces Hugo Boss",
  "description": "Costume noir en laine, parfait pour mariage",
  "type": "Suit",
  "condition": "LikeNew",
  "size": "M",
  "brand": "Hugo Boss",
  "color": "Noir",
  "dailyPrice": 50.00,
  "depositAmount": 200.00,
  "pickupAddress": "10 rue de Rivoli",
  "city": "Paris",
  "postalCode": "75001"
}
```

---

### 2. GET /api/garments/{id}
Récupérer un vêtement par ID

**Response 200:**
```json
{
  "id": "guid",
  "title": "Costume 3 pièces Hugo Boss",
  "description": "...",
  "type": "Suit",
  "dailyPrice": 50.00,
  "images": [
    { "imageUrl": "https://...", "isPrimary": true }
  ],
  "averageRating": 4.8,
  "totalReviews": 5
}
```

---

### 3. PUT /api/garments/{id}
Mettre à jour un vêtement (propriétaire uniquement)

**Headers:** `Authorization: Bearer {token}`

---

### 4. DELETE /api/garments/{id}
Supprimer un vêtement (propriétaire uniquement)

**Headers:** `Authorization: Bearer {token}`

---

### 5. GET /api/garments
Rechercher des vêtements

**Query Params:**
- `city`: Filtrer par ville
- `type`: Type de vêtement (Suit, Dress, etc.)
- `minPrice`, `maxPrice`: Fourchette de prix
- `size`: Taille

---

### 6. POST /api/garments/{id}/images
Uploader une image (max 3 par vêtement)

**Headers:** `Authorization: Bearer {token}`

**Body:** `multipart/form-data`

---

### 7. GET /api/garments/my-garments
Récupérer mes vêtements

**Headers:** `Authorization: Bearer {token}`

---

## 📅 Rentals (RentalsController)

### 1. POST /api/rentals
Créer une nouvelle réservation

**Headers:** `Authorization: Bearer {token}`

**Body:**
```json
{
  "garmentId": "guid",
  "startDate": "2025-01-20T00:00:00Z",
  "endDate": "2025-01-22T00:00:00Z"
}
```

**Response 200:**
```json
{
  "id": "guid",
  "garmentId": "guid",
  "garmentTitle": "Costume Hugo Boss",
  "startDate": "2025-01-20",
  "endDate": "2025-01-22",
  "durationDays": 3,
  "dailyPrice": 50.00,
  "totalPrice": 150.00,
  "depositAmount": 200.00,
  "status": "Pending"
}
```

---

### 2. GET /api/rentals/{id}
Récupérer une location par ID

**Headers:** `Authorization: Bearer {token}`

---

### 3. GET /api/rentals/my-rentals
Mes locations (en tant que locataire)

**Headers:** `Authorization: Bearer {token}`

---

### 4. GET /api/rentals/owner-rentals
Mes locations (en tant que propriétaire)

**Headers:** `Authorization: Bearer {token}`

---

### 5. POST /api/rentals/{id}/accept
Accepter une réservation (propriétaire)

**Headers:** `Authorization: Bearer {token}`

**Status flow:** Pending → OwnerAccepted

---

### 6. POST /api/rentals/{id}/confirm
Confirmer une réservation après paiement (locataire)

**Headers:** `Authorization: Bearer {token}`

**Status flow:** OwnerAccepted → Confirmed

**Note:** Bloque automatiquement les dates dans le calendrier de disponibilité

---

### 7. POST /api/rentals/{id}/cancel
Annuler une réservation

**Headers:** `Authorization: Bearer {token}`

**Body:**
```json
{
  "reason": "Changement de date"
}
```

**Note:** Libère automatiquement les dates dans le calendrier de disponibilité

---

## 💳 Payments (PaymentsController)

### 1. POST /api/payments/create-intent
Créer un PaymentIntent Stripe (simulation MVP)

**Headers:** `Authorization: Bearer {token}`

**Body:**
```json
{
  "rentalId": "guid",
  "type": "Rental",
  "method": "CreditCard"
}
```

**Response 200:**
```json
{
  "paymentIntentId": "pi_simulated_123",
  "clientSecret": "pi_simulated_123_secret",
  "amount": 150.00,
  "currency": "EUR",
  "status": "Pending",
  "paymentId": "guid"
}
```

---

### 2. POST /api/payments/confirm
Confirmer un paiement

**Headers:** `Authorization: Bearer {token}`

**Body:**
```json
{
  "paymentId": "guid"
}
```

---

### 3. GET /api/payments/my-payments
Mes paiements

**Headers:** `Authorization: Bearer {token}`

---

### 4. POST /api/payments/{id}/refund
Rembourser un paiement

**Headers:** `Authorization: Bearer {token}`

**Body:**
```json
{
  "amount": 150.00,
  "reason": "Annulation de la location"
}
```

---

### 5. POST /api/payments/webhook
Webhook Stripe (simulation)

---

## 💬 Conversations (ConversationsController)

### 1. POST /api/conversations
Créer ou récupérer une conversation

**Headers:** `Authorization: Bearer {token}`

**Body:**
```json
{
  "garmentId": "guid"
}
```

**Response 200:**
```json
{
  "id": "guid",
  "garmentId": "guid",
  "garmentTitle": "Costume Hugo Boss",
  "garmentImageUrl": "https://...",
  "otherParticipantId": "guid",
  "otherParticipantName": "John Doe",
  "lastMessageContent": "Bonjour, est-ce disponible ?",
  "lastMessageAt": "2025-12-11T10:30:00Z",
  "unreadCount": 2
}
```

---

### 2. GET /api/conversations
Liste de mes conversations

**Headers:** `Authorization: Bearer {token}`

**Response 200:** Array de ConversationDto

---

### 3. GET /api/conversations/{id}/messages
Historique des messages d'une conversation

**Headers:** `Authorization: Bearer {token}`

**Response 200:**
```json
[
  {
    "id": "guid",
    "senderId": "guid",
    "senderName": "John Doe",
    "content": "Bonjour, est-ce disponible pour le 25 janvier ?",
    "isRead": true,
    "readAt": "2025-12-11T10:35:00Z",
    "createdAt": "2025-12-11T10:30:00Z"
  }
]
```

---

### 4. POST /api/conversations/{id}/messages
Envoyer un message

**Headers:** `Authorization: Bearer {token}`

**Body:**
```json
{
  "content": "Oui, c'est disponible !"
}
```

---

### 5. PUT /api/conversations/{id}/read
Marquer tous les messages comme lus

**Headers:** `Authorization: Bearer {token}`

**Response 204:** No Content

---

### 6. PUT /api/conversations/messages/{messageId}/read
Marquer un message comme lu

**Headers:** `Authorization: Bearer {token}`

**Response 204:** No Content

---

## 📆 Availability (AvailabilityController)

### 1. GET /api/garments/{garmentId}/availability
Récupérer le calendrier de disponibilité (3 mois)

**Query Params:**
- `months`: Nombre de mois (1-12, default 3)

**Response 200:**
```json
{
  "garmentId": "guid",
  "garmentTitle": "Costume Hugo Boss",
  "startDate": "2025-12-11",
  "endDate": "2026-03-11",
  "availabilities": [
    {
      "date": "2025-12-11",
      "isAvailable": true,
      "blockedReason": null
    },
    {
      "date": "2025-12-25",
      "isAvailable": false,
      "blockedReason": "Rental",
      "rentalId": "guid",
      "notes": null
    },
    {
      "date": "2026-01-01",
      "isAvailable": false,
      "blockedReason": "OwnerBlocked",
      "notes": "Vacances"
    }
  ]
}
```

---

### 2. GET /api/garments/{garmentId}/availability/check
Vérifier la disponibilité d'une période

**Query Params:**
- `startDate`: Date de début (ISO 8601)
- `endDate`: Date de fin (ISO 8601)

**Response 200:**
```json
{
  "startDate": "2025-12-20",
  "endDate": "2025-12-22",
  "isAvailable": false,
  "unavailableDates": ["2025-12-21"]
}
```

---

### 3. POST /api/garments/{garmentId}/availability/block
Bloquer manuellement des dates (propriétaire)

**Headers:** `Authorization: Bearer {token}`

**Body:**
```json
{
  "startDate": "2026-01-01",
  "endDate": "2026-01-07",
  "notes": "Vacances de Noël"
}
```

**Response 204:** No Content

---

### 4. DELETE /api/garments/{garmentId}/availability/unblock
Débloquer manuellement des dates (propriétaire)

**Headers:** `Authorization: Bearer {token}`

**Body:**
```json
{
  "startDate": "2026-01-01",
  "endDate": "2026-01-07"
}
```

**Response 204:** No Content

**Note:** Ne débloque que les dates avec `BlockedReason = OwnerBlocked`

---

## 🔑 Codes d'erreur HTTP

- **200** OK - Succès
- **201** Created - Ressource créée
- **204** No Content - Succès sans contenu
- **400** Bad Request - Erreur de validation
- **401** Unauthorized - Non authentifié
- **403** Forbidden - Non autorisé
- **404** Not Found - Ressource introuvable
- **409** Conflict - Conflit (ex: dates déjà réservées)
- **500** Internal Server Error - Erreur serveur

---

## 🧪 Tests Swagger

Pour tester l'API :
1. Lancer l'API : `dotnet run` dans `SuitForU.API`
2. Ouvrir : `https://localhost:5001`
3. Autorisation :
   - Cliquer sur "Authorize"
   - Entrer : `Bearer {your-jwt-token}`
   - Valider

Voir `backend/TESTS_SWAGGER.md` pour des scénarios de test complets.
