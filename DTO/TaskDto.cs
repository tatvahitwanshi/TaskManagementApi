namespace TaskManagementApi.DTO;

public class TaskDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required string Status { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Priority { get; set; }
}
