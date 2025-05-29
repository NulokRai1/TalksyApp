import axios from "axios";
import { API_URL } from "../config";

const AuthService_URL = `${API_URL}/Auth`;

export const LoginUser = async (data) => {
  console.log("Login attempt:", data);
  try {
    const response = await axios.post(`${AuthService_URL}/login`, data, {
      headers: {
        "Content-Type": "application/json",
      },
    });

    console.log("Login success:", response.data);
    return {
      success: true,
      data: response.data,
    };
  } catch (error) {
    if (error.response) {
      console.error("Login failed:", error.response.data);
      return {
        success: false,
        error: error.response.data,
      };
    } else {
      console.error("Unexpected error:", error.message);
      return {
        success: false,
        error: "Something went wrong",
      };
    }
  }
};
