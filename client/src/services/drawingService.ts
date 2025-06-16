import axios from "axios";
import { DrawingData } from "../models/DrawingData";
import { DrawingCommand } from "../models/DrawingCommand";
const API_BASE_URL = process.env.REACT_APP_API_BASE_URL;

export const saveDrawing = async (drawing: DrawingData): Promise<number> => {
    const response = await axios.post(`${API_BASE_URL}/api/drawings`, drawing);
    return response.data.id;
};

export const getDrawing = async (id: number): Promise<DrawingData> => {
    const response = await axios.get(`${API_BASE_URL}/api/drawings/${id}`);
    return response.data;
};
export const loadDrawing = async (id: number): Promise<DrawingCommand[]> => {
    const response = await axios.get(`${API_BASE_URL}/api/drawings/${id}`);
    return JSON.parse(response.data.drawingJson);
};