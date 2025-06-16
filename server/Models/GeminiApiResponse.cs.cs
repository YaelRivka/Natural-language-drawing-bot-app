namespace Server.Models // ודא שזה ה-namespace הנכון של הפרויקט שלך
{
    // המחלקה הראשית שמייצגת את כל התשובה מה-API של ג'מיני
    public class GeminiApiResponse
    {
        public List<Candidate>? Candidates { get; set; }
        public PromptFeedback? PromptFeedback { get; set; }
    }

    // מייצגת מועמד בודד לפלט שנוצר
    public class Candidate
    {
        public Content? Content { get; set; }
        public string? FinishReason { get; set; } // לדוגמה "STOP"
        public List<SafetyRating>? SafetyRatings { get; set; }
    }

    // מייצגת את תוכן הפלט של המודל
    public class Content
    {
        public List<Part>? Parts { get; set; }
    }

    // מייצגת חלק יחיד בתוכן הפלט (בדרך כלל טקסט)
    public class Part
    {
        public string? Text { get; set; } // כאן נמצא הטקסט שאנו רוצים לחלץ
    }

    // מייצגת דירוג בטיחות עבור קטגוריה מסוימת
    public class SafetyRating
    {
        public string? Category { get; set; }
        public string? Probability { get; set; } // לדוגמה "NEGLIGIBLE", "LOW", "MEDIUM", "HIGH"
    }

    // מייצגת משוב בטיחות הקשור לפרומפט המקורי
    public class PromptFeedback
    {
        public List<SafetyRating>? SafetyRatings { get; set; }
    }
}