namespace TaskProcessing.Api.Models
{
    public class TaskProcessingLog
    {
        public int Id { get; set; }
        public int TaskItemId { get; set; }
        public TaskItem TaskItem { get; set; } = null!;
        public string Message { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

