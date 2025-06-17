import { useState } from "react";
import PromptInput from "../components/PromptInput";
import DrawingCanvas from "../components/DrawingCanvas";
import { DrawingCommand } from "../models/DrawingCommand";
import { loadDrawing, saveDrawing, getDrawing } from "../services/drawingService";
import "./css/DrawingPage.css"
const DrawingPage = () => {
    const [commands, setCommands] = useState<DrawingCommand[]>([]);
    const [prompt, setPrompt] = useState("");
    const [drawingId, setDrawingId] = useState<number | null>(null);
    const [drawingIdInput, setDrawingIdInput] = useState("");
    const [history, setHistory] = useState<DrawingCommand[][]>([]);
    const [redoStack, setRedoStack] = useState<DrawingCommand[][]>([]);

    const handleResult = (result: DrawingCommand[], usedPrompt: string) => {
        setHistory((prev) => [...prev, commands]);

        setRedoStack([]);

        setCommands((prev) => [...prev, ...result]);

        setPrompt(usedPrompt);
    };


    const handleSave = async () => {
        const data = {
            prompt,
            drawingJson: JSON.stringify(commands),
        };
        const id = await saveDrawing(data);
        setDrawingId(id);
        alert(`Saved with ID: ${id}`);
    };



    const handleLoad = async () => {
        if (!drawingIdInput.trim()) {
            alert("נא להזין מזהה ציור");
            return;
        }

        try {
            const id = parseInt(drawingIdInput);
            const loaded = await loadDrawing(id);
            setCommands(loaded);
        } catch (err) {
            console.error("שגיאה בטעינה", err);
            alert("הציור לא נמצא או הייתה שגיאה");
        }
    };
    const handleUndo = () => {
        if (history.length === 0) return;
        const newHistory = [...history];
        const lastState = newHistory.pop()!;
        setRedoStack([commands, ...redoStack]);
        setCommands(lastState);
        setHistory(newHistory);
    };
    const handleRedo = () => {
        if (redoStack.length === 0) return;
        const [nextState, ...rest] = redoStack;
        setHistory([...history, commands]);
        setCommands(nextState);
        setRedoStack(rest);
    };


    const handleNewDrawing = () => {
        setHistory([...history, commands]);
        setCommands([]);
        setRedoStack([]);
        
    }



    return (
        <div className="page-container">
            <div className="controls">
                <input
                    type="text"
                    value={drawingIdInput}
                    onChange={(e) => setDrawingIdInput(e.target.value)}
                    placeholder="מספר ציור לטעינה"
                />
                <button onClick={handleLoad}>טען ציור</button>
                <button onClick={handleSave}>שמור</button>
                <button onClick={handleUndo}>בטל</button>
                <button onClick={handleRedo}>חזור</button>
                <button onClick={handleNewDrawing}>ציור חדש</button>

            </div>

            <div className="main-content">
                <div className="left-chat">
                    <PromptInput onResult={(data) => handleResult(data, prompt)} />
                </div>
                <div className="right-canvas">
                    <DrawingCanvas commands={commands} />
                </div>
            </div>
        </div>
    );

};

export default DrawingPage;
