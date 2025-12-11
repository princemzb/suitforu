# SuitForU Frontend - Copilot Instructions

## Project Context
- [x] Clarify Project Requirements: React 18 + TypeScript + Vite for elegant wedding clothing rental platform
- [ ] Scaffold the Project
- [ ] Customize the Project
- [ ] Install Required Extensions
- [ ] Compile the Project
- [ ] Create and Run Task
- [ ] Launch the Project
- [ ] Ensure Documentation is Complete

## Project Type
React TypeScript with Vite - Elegant wedding/ceremony clothing rental platform

## Technology Stack
- React 18 + TypeScript
- Vite (build tool)
- TailwindCSS (luxury wedding theme)
- React Router DOM
- Axios (API calls)
- React Query (state management)
- React Hook Form (forms)
- Stripe React (payments)
- date-fns (dates)

## Design Theme
- **Ambiance**: Mariage, chic, élégant, luxueux
- **Palette**: Or (gold), Ivoire (ivory), Blanc cassé, Navy, Beige
- **Fonts**: Playfair Display (titres), Inter (texte)
- **Style**: Cards élégantes, animations douces, espacement généreux

## Backend Integration
- API URL: http://localhost:5001 (https://localhost:5001 en HTTPS)
- JWT Authentication avec Refresh Token Rotation
- 37 endpoints disponibles répartis sur 6 controllers:
  - Auth (7): Register, Login, Refresh, Logout, External, ConfirmEmail, Me
  - Garments (7): CRUD, Search, Upload, MyGarments
  - Rentals (7): Create, Get, MyRentals, OwnerRentals, Accept, Confirm, Cancel
  - Payments (5): CreateIntent, Confirm, MyPayments, Refund, Webhook
  - Conversations (6): Create, List, GetMessages, SendMessage, MarkRead
  - Availability (4): GetCalendar, Check, Block, Unblock

## Features to Implement
- ✅ Authentication (JWT + Refresh Token)
- ✅ Garments browsing and search
- ✅ Rental booking workflow
- ✅ Payment integration (Stripe)
- ✅ Real-time messaging (per garment)
- ✅ Availability calendar (3 months view)
- ⏳ User profile management
- ⏳ Reviews and ratings
- ⏳ Notifications
