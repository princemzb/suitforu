import axiosInstance from '../config/axios.config';
import { Payment, CreatePaymentDto, PaymentIntentDto, ProcessPaymentDto } from '../types/payment.types';
import { ApiResponse } from '../types/api.types';

export const paymentService = {
  async createPaymentIntent(data: CreatePaymentDto): Promise<PaymentIntentDto> {
    const response = await axiosInstance.post<ApiResponse<PaymentIntentDto>>(
      '/payments/create-intent',
      data
    );
    return response.data.data;
  },

  async processPayment(data: ProcessPaymentDto): Promise<Payment> {
    const response = await axiosInstance.post<ApiResponse<Payment>>(
      '/payments/process',
      data
    );
    return response.data.data;
  },

  async getMyPayments(): Promise<Payment[]> {
    const response = await axiosInstance.get<ApiResponse<Payment[]>>('/payments/my-payments');
    return response.data.data;
  },

  async refundPayment(paymentId: string): Promise<Payment> {
    const response = await axiosInstance.post<ApiResponse<Payment>>(
      `/payments/${paymentId}/refund`
    );
    return response.data.data;
  },
};
