export type ShapeType = "circle" | "rectangle" | "line" | "triangle";

export interface DrawingCommand {
  shape: ShapeType;
  x?: number;
  y?: number;
  radius?: number;
  width?: number;
  height?: number;
  from?: [number, number];
  to?: [number, number];
  color?: string;
}
