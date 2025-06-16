import { useState } from "react";
import PromptInput from "../components/PromptInput";
import DrawingCanvas from "../components/DrawingCanvas";
import { DrawingCommand } from "../models/DrawingCommand";
import { loadDrawing, saveDrawing, getDrawing } from "../services/drawingService";
const DrawingPage = () => {
    const [commands, setCommands] = useState<DrawingCommand[]>([]);
    const [prompt, setPrompt] = useState("");
    const [drawingId, setDrawingId] = useState<number | null>(null);
    const [drawingIdInput, setDrawingIdInput] = useState("");
    const [history, setHistory] = useState<DrawingCommand[][]>([]);
    const [redoStack, setRedoStack] = useState<DrawingCommand[][]>([]);

    const handleResult = (result: DrawingCommand[], usedPrompt: string) => {
        setHistory((prev) => [...prev, commands]); // שומר את המצב הנוכחי להיסטוריה
        setRedoStack([]); // איפוס ה־Redo כי זו פעולה חדשה
        setCommands(result);
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

    const handleClear = () => {
        setHistory([...history, commands]);
        setCommands([]);
        setRedoStack([]);
    };



    return (
        <div>
            <div style={{ marginTop: "10px", gap: "10px", display: "flex" }}>

                <div>
                      <input
                    type="text"
                    value={drawingIdInput}
                    onChange={(e) => setDrawingIdInput(e.target.value)}
                    placeholder="מספר ציור לטעינה"
                />
                    <button onClick={handleLoad}>טען ציור</button>
                </div>
                <button onClick={handleSave}>שמור</button>
                <button onClick={handleLoad}>טען</button>
                <button onClick={handleUndo}>בטל</button>
                <button onClick={handleRedo}>חזור</button>
                <button onClick={handleClear}>נקה</button>
            </div>
            <h2>הצ'אט שלך עם הבוט</h2>
            <PromptInput onResult={(data) => handleResult(data, prompt)} />
            <DrawingCanvas commands={commands} />
        </div>

    );
};

export default DrawingPage;
