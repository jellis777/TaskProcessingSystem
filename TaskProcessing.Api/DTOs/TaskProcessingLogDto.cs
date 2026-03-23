namespace TaskProcessing.Api.DTOs;

public class TaskProcessingLogDto
{
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}