export enum GarmentType {
  Suit = 0,
  Dress = 1,
  Shirt = 2,
  Pants = 3,
  Shoes = 4,
  Accessories = 5,
}

export enum GarmentCondition {
  New = 0,
  LikeNew = 1,
  Good = 2,
  Fair = 3,
}

export interface Garment {
  id: string;
  title: string;
  description: string;
  type: GarmentType;
  condition: GarmentCondition;
  size: string;
  brand: string;
  color: string;
  dailyPrice: number;
  depositAmount: number;
  pickupAddress: string;
  city: string;
  postalCode: string;
  country: string;
  isAvailable: boolean;
  ownerId: string;
  ownerName: string;
  viewCount: number;
  images: GarmentImage[];
  createdAt: string;
}

export interface GarmentImage {
  id: string;
  imageUrl: string;
  displayOrder: number;
  isPrimary: boolean;
}

export interface CreateGarmentDto {
  title: string;
  description: string;
  type: GarmentType;
  condition: GarmentCondition;
  size: string;
  brand: string;
  color: string;
  dailyPrice: number;
  depositAmount: number;
  pickupAddress: string;
  city: string;
  postalCode: string;
  country: string;
}

export interface GarmentSearchDto {
  city?: string;
  minPrice?: number;
  maxPrice?: number;
  type?: GarmentType;
  size?: string;
  search?: string;
  page?: number;
  pageSize?: number;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  currentPage: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}
