using System.ComponentModel.DataAnnotations;

namespace TaskNext.Api.Models;

public class TaskItem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    public bool IsDone { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Foreign Key: Ye task kis user ki hai?
    public Guid UserId { get; set; }
    public User? User { get; set; }
}