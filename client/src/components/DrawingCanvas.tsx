import React, { useEffect, useRef } from "react";
import { DrawingCommand } from "../models/DrawingCommand";
import "./css/DrawingCanvas.css"
interface DrawingCanvasProps {
  commands: DrawingCommand[];
}

const DrawingCanvas: React.FC<DrawingCanvasProps> = ({ commands }) => {
  const canvasRef = useRef<HTMLCanvasElement | null>(null);

  useEffect(() => {
    const canvas = canvasRef.current;
    if (!canvas) return;
    const ctx = canvas.getContext("2d");
    if (!ctx) return;

    ctx.clearRect(0, 0, canvas.width, canvas.height);

    commands.forEach((cmd) => {
      ctx.strokeStyle = cmd.color || "black";
      ctx.fillStyle = cmd.color || "black";
      ctx.lineWidth = 2;

      switch (cmd.shape) {
        case "circle":
          if (cmd.x != null && cmd.y != null && cmd.radius != null) {
            ctx.beginPath();
            ctx.arc(cmd.x, cmd.y, cmd.radius, 0, Math.PI * 2);
            ctx.fill();
          }
          break;

        case "rectangle":
          if (cmd.x != null && cmd.y != null && cmd.width != null && cmd.height != null) {
            ctx.fillRect(cmd.x, cmd.y, cmd.width, cmd.height);
          }
          break;

        case "line":
          if (cmd.from && cmd.to) {
            ctx.beginPath();
            ctx.moveTo(cmd.from[0], cmd.from[1]);
            ctx.lineTo(cmd.to[0], cmd.to[1]);
            ctx.stroke();
          }
          break;

        case "triangle":
          if (cmd.x != null && cmd.y != null && cmd.width != null && cmd.height != null) {
            ctx.beginPath();
            ctx.moveTo(cmd.x, cmd.y);
            ctx.lineTo(cmd.x + cmd.width / 2, cmd.y + cmd.height);
            ctx.lineTo(cmd.x - cmd.width / 2, cmd.y + cmd.height);
            ctx.closePath();
            ctx.fill();
          }
          break;

        default:
          console.warn("Unknown shape:", cmd.shape);
      }
    });
  }, [commands]);

  return (
    <canvas
      ref={canvasRef}
      width={800}
      height={600}
      style={{ border: "1px solid #ccc", marginTop: "20px" }}
    />
  );
};

export default DrawingCanvas;
