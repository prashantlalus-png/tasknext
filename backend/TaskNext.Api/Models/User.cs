using System.ComponentModel.DataAnnotations;

namespace TaskNext.Api.Models;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // One-to-Many relationship (Ek user ki bahut saari tasks ho sakti hain)
    public List<TaskItem> Tasks { get; set; } = new();
}