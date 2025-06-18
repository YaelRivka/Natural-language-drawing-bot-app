import axios from "axios";
import { DrawingCommand } from "../models/DrawingCommand";


// יצירת כתובת API מתוך מיקום הדפדפן
const API_BASE_URL =  process.env.REACT_APP_API_BASE_URL ;

export const sendPrompt = async (
  prompt: string,
  previousDrawing: DrawingCommand[]
): Promise<DrawingCommand[]> => {
  const response = await axios.post(`${API_BASE_URL}/api/prompt`, {
    prompt,
    previousDrawing, 
  });

  return response.data;
};
