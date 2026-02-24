using System.ComponentModel.DataAnnotations;

namespace TaskNext.Api.Dtos; // 'Nest' ko 'Next' kar diya

public record CreateTaskRequest(
    [Required, MaxLength(200)] string Title,
    [MaxLength(2000)] string? Description
);

public record UpdateTaskRequest(
    [Required, MaxLength(200)] string Title,
    [MaxLength(2000)] string? Description,
    bool IsDone
);