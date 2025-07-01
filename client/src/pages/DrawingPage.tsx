import { useState } from "react";
import PromptInput from "../components/PromptInput";
import DrawingCanvas from "../components/DrawingCanvas";
import { DrawingCommand } from "../models/DrawingCommand";
import { Drawing } from "../models/Drawing";
import { loadDrawing, saveDrawing, getDrawing } from "../services/drawingService";
import "./css/DrawingPage.css"
import { useEffect } from "react";
import { getUserDrawings } from "../services/drawingService"; // ודאי שמיובא
import { DrawingSummary } from "../models/Drawing";
interface DrawingPageProps {
    userId: number;
      onLogout: () => void;
}



const DrawingPage = ({ userId ,onLogout }: DrawingPageProps) => {
    const [commands, setCommands] = useState<DrawingCommand[]>([]);
    const [prompt, setPrompt] = useState("");
    const [drawingId, setDrawingId] = useState<number | null>(null);
    const [history, setHistory] = useState<DrawingCommand[][]>([]);
    const [redoStack, setRedoStack] = useState<DrawingCommand[][]>([]);
    const [resetChat, setResetChat] = useState(false);
    const [userDrawings, setUserDrawings] = useState<DrawingSummary[]>([]);
    const [selectedDrawingId, setSelectedDrawingId] = useState<number | null>(null);
    useEffect(() => {
        const fetchDrawings = async () => {
            try {
                const drawings = await getUserDrawings(userId);
                setUserDrawings(drawings);
            } catch (err) {
                console.error("שגיאה בקבלת ציורים של המשתמש", err);
            }
        };

        fetchDrawings();
    }, [userId,drawingId]);
    const handleResult = (result: DrawingCommand[], usedPrompt: string) => {
        setHistory((prev) => [...prev, commands]);

        setRedoStack([]);

        setCommands((prev) => [...prev, ...result]);

        setPrompt(usedPrompt);
    };


    const handleSave = async () => {
        const data: Drawing = {
            title: prompt, 
            userId: userId,
            commandsJson: JSON.stringify(commands)
        };
        const id = await saveDrawing(data);
        setDrawingId(id);
        alert(`Saved with ID: ${id}`);
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
        setResetChat(true);
    };
    const handleResetChatDone = () => {
        setResetChat(false);
    };



    return (
        <div className="page-container">
            <button onClick={onLogout}>התנתק</button>
            <div className="controls">
                
                <select
                    value={selectedDrawingId ?? ""}
                    onChange={(e) => setSelectedDrawingId(Number(e.target.value))}
                >
                    <option value="">בחר ציור לטעינה</option>
                    {userDrawings.map((drawing) => (
                        <option key={drawing.id} value={drawing.id}>
                            {`ציור מספר ${drawing.id}`}
                        </option>
                    ))}
                </select>
                <button
                    onClick={async () => {
                        if (!selectedDrawingId) {
                            alert("נא לבחור ציור");
                            return;
                        }
                        try {

                            const loaded = await loadDrawing(selectedDrawingId);
                            setCommands(loaded);
                        } catch (err) {
                            console.error("שגיאה בטעינה", err);
                            alert("הציור לא נמצא או הייתה שגיאה");
                        }
                    }}
                >
                    טען ציור
                </button>

                <button onClick={handleSave}>שמור</button>
                <button onClick={handleUndo}>בטל</button>
                <button onClick={handleRedo}>חזור</button>
                <button onClick={handleNewDrawing}>ציור חדש</button>

            </div>

            <div className="main-content">
                <div className="left-chat">
                    <PromptInput
                        onResult={(data) => handleResult(data, prompt)}
                        resetSignal={resetChat}
                        currentCommands={commands}
                        onResetComplete={handleResetChatDone}
                    />

                </div>
                <div className="right-canvas">
                    <DrawingCanvas commands={commands} />
                </div>
            </div>
        </div>
    );

};

export default DrawingPage;
