import React, { useState,useEffect } from "react";
import { sendPrompt } from "../services/promptService";
import { DrawingCommand } from "../models/DrawingCommand";
import "./css/PromptInput.css";
interface Message {
  sender: "user" | "bot";
  text: string;
}

const PromptInput: React.FC<{
  onResult: (commands: DrawingCommand[]) => void;
  resetSignal: boolean;
  currentCommands: DrawingCommand[];
  onResetComplete: () => void;
}> = ({ onResult, resetSignal, onResetComplete,currentCommands }) => {
  const [prompt, setPrompt] = useState("");
  const [messages, setMessages] = useState<Message[]>([]);

  useEffect(() => {
    if (resetSignal) {
      setMessages([]);  // נקה את ההודעות
      setPrompt("");    // נקה את שורת הקלט
      onResetComplete(); // עדכן להורה שסיימנו לנקות
    }
  }, [resetSignal]);


  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setPrompt("");
    if (!prompt.trim()) return;

    const userMessage: Message = { sender: "user", text: prompt };
    setMessages((prev) => [...prev, userMessage]);

    try {
const result = await sendPrompt(prompt, currentCommands);

      const botMessage: Message = {
        sender: "bot",
        text: "🎨 ציור נוסף בהצלחה",
      };

      setMessages((prev) => [...prev, botMessage]);
      onResult(result);
    } catch (err) {
      const errorMessage: Message = {
        sender: "bot",
        text: "❌ שגיאה בקבלת הציור",
      };
      setMessages((prev) => [...prev, errorMessage]);
      console.error("Error fetching drawing commands", err);
    }

    setPrompt("");
  };

  return (
    <div className="chat-container">
      <div className="chat-messages">
        {messages.map((msg, index) => (
          <div key={index} className={`message ${msg.sender}`}>
            <span className="message-content">{msg.text}</span>
          </div>
        ))}
      </div>

      <form onSubmit={handleSubmit} className="prompt-form">
        <input
          type="text"
          value={prompt}
          onChange={(e) => setPrompt(e.target.value)}
          placeholder="כתוב הוראה לציור..."
        />
        <button type="submit">שלח</button>
      </form>
    </div>
  );
};

export default PromptInput;
