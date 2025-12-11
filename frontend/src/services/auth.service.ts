import axiosInstance from '../config/axios.config';
import {
  AuthResponse,
  LoginDto,
  RegisterDto,
  RefreshTokenDto,
} from '../types/auth.types';
import { ApiResponse } from '../types/api.types';

export const authService = {
  async login(credentials: LoginDto): Promise<AuthResponse> {
    const response = await axiosInstance.post<ApiResponse<AuthResponse>>(
      '/auth/login',
      credentials
    );
    const authData = response.data.data;
    
    // Stocker les tokens
    localStorage.setItem('accessToken', authData.token);
    localStorage.setItem('refreshToken', authData.refreshToken);
    localStorage.setItem('user', JSON.stringify({
      id: authData.userId,
      email: authData.email,
      firstName: authData.firstName,
      lastName: authData.lastName,
    }));
    
    return authData;
  },

  async register(userData: RegisterDto): Promise<AuthResponse> {
    const response = await axiosInstance.post<ApiResponse<AuthResponse>>(
      '/auth/register',
      userData
    );
    const authData = response.data.data;
    
    // Stocker les tokens
    localStorage.setItem('accessToken', authData.token);
    localStorage.setItem('refreshToken', authData.refreshToken);
    localStorage.setItem('user', JSON.stringify({
      id: authData.userId,
      email: authData.email,
      firstName: authData.firstName,
      lastName: authData.lastName,
    }));
    
    return authData;
  },

  async refreshToken(refreshToken: string): Promise<AuthResponse> {
    const response = await axiosInstance.post<ApiResponse<AuthResponse>>(
      '/auth/refresh',
      { refreshToken } as RefreshTokenDto
    );
    return response.data.data;
  },

  async logout(): Promise<void> {
    const refreshToken = localStorage.getItem('refreshToken');
    if (refreshToken) {
      try {
        await axiosInstance.post('/auth/logout', { refreshToken } as RefreshTokenDto);
      } catch (error) {
        console.error('Logout error:', error);
      }
    }
    
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('user');
  },

  getCurrentUser() {
    const userStr = localStorage.getItem('user');
    return userStr ? JSON.parse(userStr) : null;
  },

  isAuthenticated(): boolean {
    return !!localStorage.getItem('accessToken');
  },
};
