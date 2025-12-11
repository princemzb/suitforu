import axiosInstance from '../config/axios.config';
import { Rental, CreateRentalDto, ExtendRentalDto, CancelRentalDto } from '../types/rental.types';
import { ApiResponse } from '../types/api.types';

export const rentalService = {
  async createRental(rentalData: CreateRentalDto): Promise<Rental> {
    const response = await axiosInstance.post<ApiResponse<Rental>>(
      '/rentals',
      rentalData
    );
    return response.data.data;
  },

  async getRentalById(id: string): Promise<Rental> {
    const response = await axiosInstance.get<ApiResponse<Rental>>(`/rentals/${id}`);
    return response.data.data;
  },

  async getMyRentals(): Promise<Rental[]> {
    const response = await axiosInstance.get<ApiResponse<Rental[]>>('/rentals/my-rentals');
    return response.data.data;
  },

  async getOwnerRentals(): Promise<Rental[]> {
    const response = await axiosInstance.get<ApiResponse<Rental[]>>('/rentals/owner-rentals');
    return response.data.data;
  },

  async acceptRental(id: string): Promise<Rental> {
    const response = await axiosInstance.post<ApiResponse<Rental>>(
      `/rentals/${id}/accept`
    );
    return response.data.data;
  },

  async confirmRental(id: string): Promise<Rental> {
    const response = await axiosInstance.post<ApiResponse<Rental>>(
      `/rentals/${id}/confirm-return`
    );
    return response.data.data;
  },

  async extendRental(id: string, data: ExtendRentalDto): Promise<Rental> {
    const response = await axiosInstance.post<ApiResponse<Rental>>(
      `/rentals/${id}/extend`,
      data
    );
    return response.data.data;
  },

  async cancelRental(id: string, data: CancelRentalDto): Promise<void> {
    await axiosInstance.post(`/rentals/${id}/cancel`, data);
  },
};
