import axiosInstance from '../config/axios.config';
import { Garment, CreateGarmentDto, GarmentSearchDto, PagedResult } from '../types/garment.types';
import { ApiResponse } from '../types/api.types';

export const garmentService = {
  async searchGarments(params: GarmentSearchDto): Promise<PagedResult<Garment>> {
    const response = await axiosInstance.get<ApiResponse<PagedResult<Garment>>>(
      '/garments',
      { params }
    );
    return response.data.data;
  },

  async getGarmentById(id: string): Promise<Garment> {
    const response = await axiosInstance.get<ApiResponse<Garment>>(`/garments/${id}`);
    return response.data.data;
  },

  async createGarment(garmentData: CreateGarmentDto): Promise<Garment> {
    const response = await axiosInstance.post<ApiResponse<Garment>>(
      '/garments',
      garmentData
    );
    return response.data.data;
  },

  async getMyGarments(): Promise<Garment[]> {
    const response = await axiosInstance.get<ApiResponse<Garment[]>>('/garments/my-garments');
    return response.data.data;
  },

  async uploadImage(garmentId: string, file: File): Promise<string> {
    const formData = new FormData();
    formData.append('file', file);

    const response = await axiosInstance.post<ApiResponse<string>>(
      `/garments/${garmentId}/images`,
      formData,
      {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      }
    );
    return response.data.data;
  },
};
