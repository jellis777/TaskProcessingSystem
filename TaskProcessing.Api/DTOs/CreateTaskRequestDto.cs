
using System.ComponentModel.DataAnnotations;

namespace TaskProcessing.Api.DTOs
{
    public class CreateTaskRequestDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Type { get; set; } = string.Empty;

        [StringLength(5000)]
        public string? PayloadJson { get; set; }
    }
}