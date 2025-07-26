import api from './api';
import { LoginRequest, RegisterRequest, User } from '../types';

interface AuthResponse {
  token: string;
  id: number;
  email: string;
  name: string;
  timeCredits: number;
  profileImageUrl?: string;
}

const authService = {
  async login(credentials: LoginRequest): Promise<AuthResponse> {
    const response = await api.post<AuthResponse>('/auth/login', credentials);
    return response.data;
  },

  async register(userData: RegisterRequest): Promise<AuthResponse> {
    const response = await api.post<AuthResponse>('/auth/register', userData);
    return response.data;
  },

  async getCurrentUser(): Promise<User> {
    const response = await api.get<User>('/auth/me');
    return response.data;
  },

  async logout(): Promise<void> {
    await api.post('/auth/logout');
  },
};

export default authService;