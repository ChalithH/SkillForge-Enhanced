export interface User {
  id: number;
  email: string;
  name: string;
  timeCredits: number;
  bio?: string;
  profileImageUrl?: string;
}

export interface Skill {
  id: number;
  name: string;
  category: string;
  description: string;
}

export interface UserSkill {
  id: number;
  userId: number;
  skillId: number;
  proficiencyLevel: number;
  isOffering: boolean;
  description?: string;
  skill?: Skill;
}

export interface AuthState {
  user: User | null;
  token: string | null;
  isLoading: boolean;
  error: string | null;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  name: string;
}

export interface ProfileUpdateData {
  name: string;
  bio?: string;
  profileImageUrl?: string;
}