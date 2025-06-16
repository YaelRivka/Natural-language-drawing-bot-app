import { DrawingCommand } from "./DrawingCommand";

export interface PromptInputProps {
  onResult: (data: DrawingCommand[],propmt:string) => void;
}
