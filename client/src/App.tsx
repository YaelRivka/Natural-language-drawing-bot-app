import { useState } from "react";
import AuthForm from "./components/AuthForm";
import DrawingPage from "./pages/DrawingPage";
import { AuthService } from "./services/AuthService";

const App = () => {
  const [userId, setUserId] = useState<number | null>(() => {
      const id = AuthService.getUserId();
      return id ? Number(id) : null;
  });

  const handleLogout = () => {
      AuthService.logout();
      setUserId(null);
  };

  return userId ? (
      <DrawingPage userId={userId} onLogout={handleLogout} />
  ) : (
    <AuthForm onAuthSuccess={(id: string) => setUserId(Number(id))} />
  );
};

export default App;
