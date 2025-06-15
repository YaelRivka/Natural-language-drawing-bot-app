

import { useState } from "react";
import PromptInput from "../components/PromptInput";
import DrawingCanvas from "../components/DrawingCanvas";
import { DrawingCommand } from "../models/DrawingCommand";

const DrawingPage = () => {
  const [commands, setCommands] = useState<DrawingCommand[]>([]);

  const handleResult = (result: DrawingCommand[]) => {
    setCommands(result);
  };

  return (
    <div>
      <h2>צייר לפי הוראה</h2>
      <PromptInput onResult={handleResult} />
      <DrawingCanvas commands={commands} />
    </div>
  );
};

export default DrawingPage;
