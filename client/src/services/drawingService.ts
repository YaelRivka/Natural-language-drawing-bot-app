import axios from "axios";
import { DrawingData } from "../models/DrawingData";
import { DrawingCommand } from "../models/DrawingCommand";
import { Drawing } from "../models/Drawing";
import { DrawingSummary } from "../models/Drawing";
const API_BASE_URL = process.env.REACT_APP_API_BASE_URL;

// export const saveDrawing = async (drawing: DrawingData): Promise<number> => {
//     const response = await axios.post(`${API_BASE_URL}/api/drawings`, drawing);
//     return response.data.id;
// };
export const saveDrawing = async (drawing: Drawing): Promise<number> => {
  const response = await axios.post(`${API_BASE_URL}/api/drawings`, {
    ...drawing,
    commandsJson: JSON.stringify(drawing.commandsJson), // חשוב מאוד
  });
  return response.data.id;
};
export const getUserDrawings = async (userId: number): Promise<DrawingSummary[]> => {
    const response = await axios.get(`${API_BASE_URL}/api/drawings/user/${userId}`);
    return response.data;
};
export const getDrawing = async (id: number): Promise<DrawingData> => {
    const response = await axios.get(`${API_BASE_URL}/api/drawings/${id}`);
    return response.data;
};
// export const loadDrawing = async (id: number): Promise<DrawingCommand[]> => {
//   const response = await axios.get(`${API_BASE_URL}/api/drawings/${id}`);

//   const doubleEncoded = response.data.commandsJson;
//   const onceDecoded = JSON.parse(doubleEncoded);  // מוריד את המחרוזת הפנימית
//   return onceDecoded as DrawingCommand[];         // מחזיר כ-array
// };
export const loadDrawing = async (id: number): Promise<DrawingCommand[]> => {
  const response = await axios.get(`${API_BASE_URL}/api/drawings/${id}`);
  
  const doubleEncoded = response.data.commandsJson;   // מחרוזת עם בריחות
  const onceDecoded = JSON.parse(doubleEncoded);       // מחרוזת JSON רגילה (של מערך)
  const commandsArray = JSON.parse(onceDecoded);       // מערך האובייקטים בפועל

  return commandsArray;
};

