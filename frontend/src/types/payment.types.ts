export interface Payment {
  id: string;
  rentalId: string;
  amount: number;
  status: string;
  createdAt: string;
}

export interface CreatePaymentDto {
  rentalId: string;
  type: number;
  method: number;
  paymentToken?: string | null;
}

export interface PaymentIntentDto {
  paymentIntentId: string;
  clientSecret: string;
  amount: number;
  currency: string;
  status: string;
  paymentId: string;
}

export interface ProcessPaymentDto {
  paymentId: string;
  paymentIntentId: string;
}
