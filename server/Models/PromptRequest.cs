namespace Server.Models
{
    public class PromptRequest
    {
        public string Prompt { get; set; } = string.Empty;
        public List<DrawingCommand> ?PreviousDrawing { get; set; }
    }
}
