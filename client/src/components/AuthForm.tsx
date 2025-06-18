import { useState } from "react";
import { AuthService } from "../services/AuthService";
import "./css/AuthForm.css"
import { Users } from "../models/Users";
const AuthForm = ({ onAuthSuccess }: { onAuthSuccess: (userId: string) => void }) => {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [isLogin, setIsLogin] = useState(true);
    const [message, setMessage] = useState("");

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        const user: Users = { id: 0, email: "", PasswordHash: "" }
        user.email = email;

        user.PasswordHash = password;
        try {

            const userId = isLogin
                ? await AuthService.login(user)
                : await AuthService.register(user);

            onAuthSuccess(userId);
        } catch (err: any) {
            // אם data הוא אובייקט - נהפוך אותו למחרוזת JSON
            const errorData = err.response?.data;
            const errorMessage =
                typeof errorData === "string"
                    ? errorData
                    : errorData && typeof errorData === "object"
                        ? JSON.stringify(errorData)
                        : "שגיאה בהתחברות/רישום";

            setMessage(errorMessage);

        }
    };

    return (
        <div className="auth-form">
            <h2>{isLogin ? "התחברות" : "הרשמה"}</h2>
            <form onSubmit={handleSubmit}>
                <input
                    type="email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    placeholder="אימייל"
                    required
                />
                <input
                    type="password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    placeholder="סיסמה"
                    required
                />
                <button type="submit">{isLogin ? "התחבר" : "הרשם"}</button>
                <button onClick={() => setIsLogin(!isLogin)}>
                    {isLogin ? "אין לך חשבון? הרשם" : "יש לך חשבון? התחבר"}
                    <div className="white"></div>
                </button>
                {message && <p className="message">{message}</p>}
            </form>
          
            </div>
      
    );
};

export default AuthForm;
