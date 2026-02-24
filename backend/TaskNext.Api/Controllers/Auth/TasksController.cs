using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskNext.Api.Data;
using TaskNext.Api.Dtos;
using TaskNext.Api.Models;

namespace TaskNext.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] 
public class TasksController : ControllerBase
{
    private readonly AppDbContext _db;

    public TasksController(AppDbContext db)
    {
        _db = db;
    }

    // 1. Helper: Token se Guid nikalne ke liye
    private Guid GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier) 
                    ?? User.FindFirst("sub");

        if (claim == null) throw new Exception("Unauthorized: User ID claim missing.");

        return Guid.Parse(claim.Value); 
    }

    // 2. GET: Saare tasks fetch karne ke liye
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskItem>>> GetMyTasks()
    {
        var userId = GetUserId(); // Ab ye Guid hai
        var tasks = await _db.Tasks
            .Where(t => t.UserId == userId) // Guid == Guid (Success)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return Ok(tasks);
    }

    // 3. POST: Naya task banane ke liye
    [HttpPost]
    public async Task<ActionResult<TaskItem>> Create(CreateTaskRequest req)
    {
        var userId = GetUserId();

        var task = new TaskItem
        {
            Title = req.Title.Trim(),
            Description = req.Description?.Trim(),
            UserId = userId, // Guid assignment (Success)
            CreatedAt = DateTime.UtcNow
        };

        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();

        return Ok(task);
    }

    // 4. PUT: Task update karne ke liye
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TaskItem>> Update(Guid id, UpdateTaskRequest req)
    {
        var userId = GetUserId();
        // Database mein search karte waqt dono IDs Guid honi chahiye
        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

        if (task is null) return NotFound();

        task.Title = req.Title.Trim();
        task.Description = req.Description?.Trim();
        task.IsDone = req.IsDone;

        await _db.SaveChangesAsync();
        return Ok(task);
    }

    // 5. DELETE: Task hatane ke liye
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetUserId();
        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

        if (task is null) return NotFound();

        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}