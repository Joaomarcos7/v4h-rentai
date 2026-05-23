export type UserRole = 'Solicitante' | 'Especialista';

export interface AuthResult {
  token: string;
  userId: string;
  name: string;
  email: string;
  role: UserRole;
}
