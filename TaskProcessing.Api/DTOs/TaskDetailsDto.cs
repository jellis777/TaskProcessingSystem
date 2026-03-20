

using TaskProcessing.Api.Enums;

namespace TaskProcessing.Api.DTOs
{
    public class TaskDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public Status Status { get; set; }

        public string? PayloadJson { get; set; }

        public string? ResultJson { get; set; }

        public string? ErrorMessage { get; set; }

        public int RetryCount { get; set; }

        public int MaxRetries { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime? StartedAt { get; set; }

        public DateTime? CompletedAt { get; set; }
    }
}