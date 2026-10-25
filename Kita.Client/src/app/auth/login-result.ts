import { User } from "./user.model";

export interface LoginResult {
  success: boolean;
  message: string;
  token?: string;
  user?: User;
}
