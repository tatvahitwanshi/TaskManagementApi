using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementApi.Models;

namespace TaskManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaskController : ControllerBase
{
    private readonly TaskManagementDbContext _context;

    public TaskController(TaskManagementDbContext context)
    {
        _context = context;
    }

    [HttpGet("by-user")]
    [Authorize]
    public async Task<IActionResult> GetTasksForUser()
    {
        var allTasks = await _context.Tasks.ToListAsync();
        return Ok(allTasks);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateTask(TaskManagementApi.Models.Task task)
    {
        task.Createdat = DateTime.Now;
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return Ok(task);
    }
    private int GetUserIdFromToken()
    {
        int userId = int.Parse(User.FindFirst("UserId")!.Value);
        return userId;
    }

    private string GetUserRoleFromToken()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value!;
    }

[HttpGet("{id}")]
public async Task<IActionResult> GetTask(int id)
{
    var task = await _context.Tasks.FindAsync(id);
    if (task == null) return NotFound();
    return Ok(task);
}

[HttpPut("{id}")]
public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskManagementApi.Models.Task updatedTask)
{
    var task = await _context.Tasks.FindAsync(id);
    if (task == null) return NotFound();

    task.Title = updatedTask.Title;
    task.Description = updatedTask.Description;
    task.Status = updatedTask.Status;
    task.Duedate = updatedTask.Duedate;
    task.Priority = updatedTask.Priority;

    await _context.SaveChangesAsync();
    return NoContent();
}


}
