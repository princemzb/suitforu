export enum RentalStatus {
  Pending = 0,
  OwnerAccepted = 1,
  Confirmed = 2,
  Active = 3,
  Completed = 4,
  Cancelled = 5,
}

export interface Rental {
  id: string;
  garmentId: string;
  renterId: string;
  ownerId: string;
  startDate: string;
  endDate: string;
  durationDays: number;
  dailyPrice: number;
  totalPrice: number;
  depositAmount: number;
  status: RentalStatus;
  ownerAcceptedAt?: string;
  renterConfirmedAt?: string;
  cancellationReason?: string;
  createdAt: string;
}

export interface CreateRentalDto {
  garmentId: string;
  startDate: string;
  endDate: string;
}

export interface ExtendRentalDto {
  newEndDate: string;
}

export interface CancelRentalDto {
  reason: string;
}
