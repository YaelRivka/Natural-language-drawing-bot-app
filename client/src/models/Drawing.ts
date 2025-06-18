export interface Drawing {
  id?: number;
  title?: string;
  userId?: number;
  commandsJson: string;
  createdAt?: string;
}
export interface DrawingSummary {
  id: number;
}