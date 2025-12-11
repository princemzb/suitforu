# Configuration Stripe - SuitForU

## 📋 Vue d'ensemble

L'intégration Stripe est maintenant complète dans le backend. Ce document explique comment configurer et tester le système de paiement.

## 🔑 Obtenir les clés API Stripe

### 1. Créer un compte Stripe

1. Allez sur [stripe.com](https://stripe.com)
2. Créez un compte (gratuit en mode test)
3. Vérifiez votre email

### 2. Récupérer les clés de test

1. Connectez-vous au [Dashboard Stripe](https://dashboard.stripe.com)
2. En haut à droite, assurez-vous d'être en **mode Test** (toggle "Test mode")
3. Allez dans **Developers** → **API keys**
4. Copiez :
   - **Publishable key** : `pk_test_...`
   - **Secret key** : `sk_test_...` (cliquez sur "Reveal test key")

### 3. Créer un Webhook Secret

1. Dans le Dashboard, allez dans **Developers** → **Webhooks**
2. Cliquez sur **Add endpoint**
3. URL de l'endpoint : `https://localhost:5156/api/payments/webhook`
4. Événements à écouter :
   - `payment_intent.succeeded`
   - `payment_intent.payment_failed`
   - `charge.refunded`
5. Cliquez sur **Add endpoint**
6. Copiez le **Signing secret** : `whsec_...`

## ⚙️ Configuration Backend

### Mettre à jour appsettings.Development.json

Remplacez les valeurs placeholder dans `backend/src/SuitForU.API/appsettings.Development.json` :

```json
{
  "Stripe": {
    "PublishableKey": "pk_test_VOTRE_CLE_PUBLIQUE",
    "SecretKey": "sk_test_VOTRE_CLE_SECRETE",
    "WebhookSecret": "whsec_VOTRE_WEBHOOK_SECRET"
  }
}
```

⚠️ **Important** : Ne jamais commiter les vraies clés dans Git !

## 🧪 Tester l'intégration

### 1. Cartes de test Stripe

Utilisez ces numéros de carte pour les tests :

| Carte | Numéro | Résultat |
|-------|--------|----------|
| ✅ Succès | `4242 4242 4242 4242` | Paiement réussi |
| ❌ Décliné | `4000 0000 0000 9995` | Carte insuffisamment approvisionnée |
| 🔐 Authentification | `4000 0025 0000 3155` | Requiert 3D Secure |

- **Date d'expiration** : N'importe quelle date future (ex: 12/25)
- **CVC** : N'importe quel 3 chiffres (ex: 123)
- **Code postal** : N'importe quel code (ex: 12345)

### 2. Workflow de paiement

#### A. Créer un PaymentIntent

```http
POST /api/payments/create-intent
Authorization: Bearer {jwt_token}
Content-Type: application/json

{
  "rentalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

Réponse :
```json
{
  "success": true,
  "data": {
    "paymentIntentId": "pi_3NqZ...",
    "clientSecret": "pi_3NqZ..._secret_...",
    "amount": 150.00,
    "currency": "eur",
    "status": "requires_payment_method"
  }
}
```

#### B. Confirmer le paiement (après succès côté client)

```http
POST /api/payments/confirm
Authorization: Bearer {jwt_token}
Content-Type: application/json

{
  "paymentIntentId": "pi_3NqZ..."
}
```

#### C. Rembourser un paiement

```http
POST /api/payments/{paymentId}/refund
Authorization: Bearer {jwt_token}
```

### 3. Tester les webhooks localement

#### Installer Stripe CLI

```powershell
# Windows (avec Chocolatey)
choco install stripe-cli

# Ou télécharger depuis https://github.com/stripe/stripe-cli/releases
```

#### Écouter les webhooks

```powershell
# Se connecter
stripe login

# Rediriger les webhooks vers le serveur local
stripe listen --forward-to https://localhost:5156/api/payments/webhook
```

La CLI affichera un **webhook signing secret** temporaire : `whsec_...`
Utilisez-le dans `appsettings.Development.json` pour les tests locaux.

#### Déclencher un webhook de test

```powershell
stripe trigger payment_intent.succeeded
```

## 🔍 Logs et Debug

### Vérifier les logs

```csharp
// Les logs apparaissent dans la console du backend :
[INF] Payment intent created for rental {RentalId}, PaymentIntent {PaymentIntentId}
[INF] Payment processed successfully for rental {RentalId}, Charge {ChargeId}
[INF] Stripe webhook received: payment_intent.succeeded
```

### Dashboard Stripe

1. Allez dans **Payments** → **All payments** pour voir les transactions
2. Allez dans **Developers** → **Events** pour voir tous les webhooks
3. Cliquez sur un événement pour voir les détails et réessayer si nécessaire

## 🏗️ Architecture

### Services Stripe utilisés

- **PaymentIntentService** : Créer et récupérer des PaymentIntents
- **RefundService** : Créer des remboursements
- **EventUtility** : Valider la signature des webhooks

### Flux de paiement

```
1. Client → POST /create-intent → Backend
   ↓
2. Backend → Stripe API : Create PaymentIntent
   ↓
3. Backend → Client : { clientSecret }
   ↓
4. Client → Stripe.js : confirmPayment(clientSecret)
   ↓
5. Stripe → Webhook → Backend : payment_intent.succeeded
   ↓
6. Client → POST /confirm → Backend
   ↓
7. Backend → Stripe API : Get PaymentIntent (verify status)
   ↓
8. Backend → Database : Update payment status
```

## 📊 États des paiements

| Status | Description |
|--------|-------------|
| `Pending` | PaymentIntent créé, attente de paiement |
| `Succeeded` | Paiement confirmé avec succès |
| `Failed` | Paiement échoué |
| `Refunded` | Paiement remboursé |
| `PartiallyRefunded` | Remboursement partiel |

## 🚀 Passage en Production

### Avant de déployer

1. ⚠️ **Passer en mode Live** dans le Dashboard Stripe
2. Récupérer les **vraies clés** (commencent par `pk_live_...` et `sk_live_...`)
3. Créer un nouveau **webhook endpoint** avec l'URL de production
4. Mettre à jour `appsettings.Production.json` avec les clés live
5. ⚠️ **Sauvegarder les clés** dans un gestionnaire de secrets (Azure Key Vault, AWS Secrets Manager)
6. Vérifier la conformité PCI-DSS (Stripe s'en charge si vous utilisez Stripe.js)

### Variables d'environnement (recommandé)

```bash
# Production
export STRIPE_SECRET_KEY="sk_live_..."
export STRIPE_PUBLISHABLE_KEY="pk_live_..."
export STRIPE_WEBHOOK_SECRET="whsec_..."
```

Puis dans `appsettings.Production.json` :
```json
{
  "Stripe": {
    "SecretKey": "${STRIPE_SECRET_KEY}",
    "PublishableKey": "${STRIPE_PUBLISHABLE_KEY}",
    "WebhookSecret": "${STRIPE_WEBHOOK_SECRET}"
  }
}
```

## 🔐 Sécurité

### ✅ Bonnes pratiques implémentées

- Validation de la signature webhook avec `EventUtility.ConstructEvent`
- Clés API stockées dans configuration (ne jamais commiter)
- Montants en centimes pour éviter les erreurs d'arrondi
- Gestion d'erreurs Stripe avec `StripeException`
- Logs détaillés pour le debug et l'audit

### ⚠️ À ne jamais faire

- ❌ Commiter les clés API dans Git
- ❌ Utiliser les clés live en développement
- ❌ Exposer le Secret Key côté client
- ❌ Accepter les webhooks sans validation de signature
- ❌ Calculer les montants côté client (toujours côté serveur)

## 📚 Ressources

- [Documentation Stripe .NET](https://stripe.com/docs/api?lang=dotnet)
- [Stripe Testing](https://stripe.com/docs/testing)
- [Webhooks Best Practices](https://stripe.com/docs/webhooks/best-practices)
- [Cartes de test](https://stripe.com/docs/testing#cards)
- [Stripe CLI](https://stripe.com/docs/stripe-cli)

## 🐛 Troubleshooting

### "Invalid API Key provided"

- Vérifiez que la clé dans `appsettings.Development.json` est correcte
- Assurez-vous d'utiliser la clé **Secret** (commence par `sk_test_`)
- Redémarrez le serveur après modification

### "No such payment_intent"

- Le PaymentIntent n'existe pas dans votre compte Stripe
- Vérifiez que vous êtes en mode Test
- Utilisez le Dashboard pour vérifier les PaymentIntents créés

### Webhook signature verification failed

- Le webhook secret est incorrect
- Utilisez `stripe listen` pour obtenir un secret temporaire en dev
- En production, copiez le secret depuis le Dashboard → Webhooks

### Payment status not updating

- Vérifiez que le webhook est bien reçu (logs backend)
- Testez avec `stripe trigger payment_intent.succeeded`
- Vérifiez l'URL du webhook dans le Dashboard Stripe
