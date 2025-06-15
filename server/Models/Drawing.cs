namespace Server.Models
{
    public class Drawing
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? UserId { get; set; } // בעתיד
        public string CommandsJson { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
