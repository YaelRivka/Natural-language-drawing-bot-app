import axios from "axios";
import { Users } from "../models/Users";
const API_BASE_URL = process.env.REACT_APP_API_BASE_URL;

export const AuthService = {
  async register(user:Users) {
    const response = await axios.post(`${API_BASE_URL}/api/auth/register`, user );
    return (response.data.userId);
  },

  async login(user:Users) {
    const response = await axios.post(`${API_BASE_URL}/api/auth/login`,  user);
    return response.data.userId;
  },

  logout() {
    localStorage.removeItem("userId");
  },

  getUserId(): string | null {
    return localStorage.getItem("userId");
  },

  isLoggedIn(): boolean {
    return !!localStorage.getItem("userId");
  }
};
