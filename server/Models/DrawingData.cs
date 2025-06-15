namespace Server.Models
{
    public class DrawingData
    {
        public int Id { get; set; }
        public string Prompt { get; set; }
        public string DrawingJson { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
