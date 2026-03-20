

using TaskProcessing.Api.Enums;

namespace TaskProcessing.Api.DTOs
{
    public class TaskSummaryDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public Status Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}