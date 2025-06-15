import React, { useState } from "react";
import { sendPrompt } from "../services/promptService";
import { PromptInputProps } from "../models/PromptInputProps";

const PromptInput: React.FC<PromptInputProps> = ({ onResult }) => {
  const [prompt, setPrompt] = useState("");

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const data = await sendPrompt(prompt);
      onResult(data);
    } catch (err) {
      console.error("Error fetching drawing commands", err);
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <input
        type="text"
        value={prompt}
        onChange={(e) => setPrompt(e.target.value)}
        placeholder="כתוב הוראה לציור..."
      />
      <button type="submit">צייר</button>
    </form>
  );
};

export default PromptInput;
