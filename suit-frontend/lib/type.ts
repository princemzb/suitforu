export type User = {
    id: string;
    email: string;
    firstName: string;
    lastName: string;
    profilePictureUrl?: string;
  };
  
  export type GarmentType = 'Suit' | 'Jacket' | 'Tuxedo' | 'Waistcoat';
 
  
  export type GarmentCondition = 'Like New' | 'Excellent' | 'Good' | 'Vintage';

  export type Garment = {
    id: string;
    ownerId: string;
    title: string;
    description: string;
    type: string; // Suit, Tuxedo, etc.
    size: string;
    brand: string;
    dailyPrice: number;
    images: string[];
    city: string;
    isAvailable: boolean;
    condition: GarmentCondition; // Ensure this exists
    createdAt: Date; // For sorting by "Newest"
  };