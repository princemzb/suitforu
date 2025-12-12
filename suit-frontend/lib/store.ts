import { create } from 'zustand';
import { Garment, User } from './type';
import { persist } from 'zustand/middleware';

export type RentalStatus = 'PENDING' | 'APPROVED' | 'REJECTED' | 'PAID' | 'PICKED_UP' | 'RETURNED' | 'COMPLETED';
export interface Rental {
  id: string; // Unique order ID
  garmentId: string;
  startDate: string;
  days: number;
  renterName: string;
  totalPrice: number;
  status: RentalStatus;
  qrCodeUrl: string; // Pour la récupération
  orderDate: Date;
}


export interface Review {
  id: string;
  garmentId: string;
  reviewerName: string;
  reviewerImage: string;
  rating: number; // 1-5
  comment: string;
  date: string;
}


interface AppState {
  currentUser: User | null;
  garments: Garment[];
  login: (email: string) => void;
  wishlist: string[];
  logout: () => void; // Add this
  getOwner: (id: string) => PublicUser | undefined;
  addGarment: (garment: any) => void;
  toggleWishlist: (id: string) => void;
  rentals: Rental[]; 
  addRental: (rental: Rental) => void;
  updateRentalStatus: (rentalId: string, status: RentalStatus) => void;
  reviews: Review[];
  toggleAvailability: (garmentId: string) => void;
}

export interface PublicUser {
  id: string;
  firstName: string;
  lastName: string;
  profilePictureUrl: string;
  isVerified: boolean;
  rating: number;
  reviews: number;
  responseTime: string; // e.g. "within 1 hour"
  joinedDate: string;
}

export const useAppStore = create<AppState>((set) => ({
  toggleAvailability: (garmentId) => set((state) => ({
    garments: state.garments.map((g) => g.id === garmentId ? { ...g, isAvailable: !g.isAvailable } : g)
  })),
  reviews: [
    { id: 'rv1', garmentId: '1', reviewerName: 'James B.', reviewerImage: '...', rating: 5, comment: 'Exceptional quality. Fit perfectly.', date: '2 Dec 2023' },
    { id: 'rv2', garmentId: '1', reviewerName: 'Tony S.', reviewerImage: '...', rating: 4, comment: 'Great suit but sleeves were slightly long.', date: '15 Nov 2023' },
  ],
  rentals:[{
    id: 'ord_123',
    garmentId: '1',
    startDate: '2023-12-25',
    days: 3,
    totalPrice: 650,
    status: 'PAID', // État initial
    qrCodeUrl: 'dummy-qr',
    orderDate: new Date(),
    renterName: 'Alexander Pierce',
  }],
  addRental: (rental) => set((state) => ({ rentals: [rental, ...state.rentals] })),
  updateRentalStatus: (rentalId, status) => set((state) => ({
    rentals: state.rentals.map((r) => r.id === rentalId ? { ...r, status } : r)
  })),
  wishlist: [],
  toggleWishlist: (id) => set((state) => {
    const exists = state.wishlist.includes(id);
    return {
      wishlist: exists 
        ? state.wishlist.filter((itemId) => itemId !== id) // Remove
        : [...state.wishlist, id] // Add
    };
  }),
  garments: [
    {
      id: '1',
      ownerId: 'u1',
      title: 'Midnight Blue Tuxedo',
      description: 'A classic dinner jacket for high-end events. Worn once.',
      type: 'Tuxedo',
      size: '40R',
      brand: 'Tom Ford',
      dailyPrice: 200,
      city: 'London',
      images: ['https://images.unsplash.com/photo-1593030761757-71bd90d24359?ixlib=rb-4.0.3&auto=format&fit=crop&w=1000&q=80'],
      isAvailable: true,
      condition: 'Like New', 
      createdAt: new Date('2023-12-01'),
    },
    {
      id: '2',
      ownerId: 'u2',
      title: 'Charcoal Pinstripe Three-Piece',
      description: 'Perfect for closing deals or winter weddings.',
      type: 'Suit',
      size: '42L',
      brand: 'Brioni',
      dailyPrice: 150,
      city: 'New York',
      images: ['https://images.unsplash.com/photo-1594938298603-c8148c47e356?ixlib=rb-4.0.3&auto=format&fit=crop&w=1000&q=80'],
      isAvailable: true,
      condition: 'Excellent', 
       createdAt: new Date('2023-12-01'),
    },
    {
      id: '3',
      ownerId: 'u3',
      title: 'Velvet Dinner Jacket',
      description: 'Burgundy velvet, shawl collar. A statement piece.',
      type: 'Jacket',
      size: '38R',
      brand: 'Ralph Lauren',
      dailyPrice: 120,
      city: 'Paris',
      images: ['https://images.unsplash.com/photo-1555066931-4365d14bab8c?ixlib=rb-4.0.3&auto=format&fit=crop&w=1000&q=80'],
      isAvailable: true,
      condition: 'Like New', 
      createdAt: new Date('2023-12-01'),
    },
    {
      id: '4',
      ownerId: 'u4',
      title: 'Beige Summer Linen Suit',
      description: 'Lightweight, perfect for destination weddings.',
      type: 'Suit',
      size: '40R',
      brand: 'Suitsupply',
      dailyPrice: 90,
      city: 'Miami',
      images: ['https://images.unsplash.com/photo-1487222477894-8943e31ef7b2?ixlib=rb-4.0.3&auto=format&fit=crop&w=1000&q=80'],
      isAvailable: true,
      condition: 'Like New', 
      createdAt: new Date('2023-12-01'),
    },
  ],

  login: (email) => {
    set({ 
      currentUser: { 
        id: 'user_123', 
        email: email, 
        firstName: 'Alexander', 
        lastName: 'Pierce',
        profilePictureUrl: 'https://images.unsplash.com/photo-1500648767791-00dcc994a43e?auto=format&fit=crop&w=200&q=80'
      } 
    });
  },
  getOwner: (id) => ({id:"1", firstName:"Alexander", lastName:"Pierce", profilePictureUrl:"https://images.unsplash.com/photo-1500648767791-00dcc994a43e?auto=format&fit=crop&w=200&q=80", isVerified:true, rating:4.8, reviews:34, responseTime:"within 1 hour", joinedDate:"January 2022"}),

  // Simulate Logout
  logout: () => set({ currentUser: null }),
  currentUser: { 
        id: 'user_123', 
        email: "ad@gmail.com", 
        firstName: 'Alexander', 
        lastName: 'Pierce',
        profilePictureUrl: 'https://images.unsplash.com/photo-1500648767791-00dcc994a43e?auto=format&fit=crop&w=200&q=80'
      } ,
  addGarment: (garment) => {

    console.log('Garment added:', garment);

    // Add logic to update the store or make an API call

  },
}));