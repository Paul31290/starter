export interface User {
  id: number;
  userName: string;
  email: string;
  phone?: string;
  avatarUrl?: string;
  profilePicture?: string;
  firstName?: string;
  lastName?: string;
  createdAt: Date;
  lastLoginAt?: Date;
  isActive: boolean;
  roles: string[];
}