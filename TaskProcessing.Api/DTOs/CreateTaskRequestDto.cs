
namespace TaskProcessing.Api.DTOs
{
    public class CreateTaskRequestDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? PayloadJson { get; set; }
    }
}