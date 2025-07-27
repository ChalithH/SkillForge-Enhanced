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

export interface CreateUserSkillRequest {
  skillId: number;
  proficiencyLevel: number;
  isOffering: boolean;
  description?: string;
}

export enum ExchangeStatus {
  Pending = 0,
  Accepted = 1,
  Rejected = 2,
  Cancelled = 3,
  Completed = 4,
  NoShow = 5
}

export interface SkillExchange {
  id: number;
  offererId: number;
  offerer?: User;
  learnerId: number;
  learner?: User;
  skillId: number;
  skill?: Skill;
  scheduledAt: string;
  duration: number;
  status: ExchangeStatus;
  meetingLink?: string;
  notes?: string;
  createdAt: string;
  updatedAt: string;
  reviews: Review[];
}

export interface Review {
  id: number;
  exchangeId: number;
  exchange?: SkillExchange;
  reviewerId: number;
  reviewer?: User;
  reviewedUserId: number;
  reviewedUser?: User;
  rating: number;
  comment?: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateExchangeRequest {
  offererId: number;
  learnerId: number;
  skillId: number;
  scheduledAt: string;
  duration: number;
  meetingLink?: string;
  notes?: string;
}