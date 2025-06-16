namespace Server.Models // ��� ��� �-namespace ����� �� ������� ���
{
    // ������ ������ ������� �� �� ������ ��-API �� �'����
    public class GeminiApiResponse
    {
        public List<Candidate>? Candidates { get; set; }
        public PromptFeedback? PromptFeedback { get; set; }
    }

    // ������ ����� ���� ���� �����
    public class Candidate
    {
        public Content? Content { get; set; }
        public string? FinishReason { get; set; } // ������ "STOP"
        public List<SafetyRating>? SafetyRatings { get; set; }
    }

    // ������ �� ���� ���� �� �����
    public class Content
    {
        public List<Part>? Parts { get; set; }
    }

    // ������ ��� ���� ����� ���� (���� ��� ����)
    public class Part
    {
        public string? Text { get; set; } // ��� ���� ����� ���� ����� ����
    }

    // ������ ����� ������ ���� ������� ������
    public class SafetyRating
    {
        public string? Category { get; set; }
        public string? Probability { get; set; } // ������ "NEGLIGIBLE", "LOW", "MEDIUM", "HIGH"
    }

    // ������ ���� ������ ����� ������� ������
    public class PromptFeedback
    {
        public List<SafetyRating>? SafetyRatings { get; set; }
    }
}