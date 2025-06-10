using System;
using System.Collections.Generic;

namespace TaskManagementApi.Models;

public partial class Task
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string Status { get; set; } = null!;

    public DateOnly? Duedate { get; set; }

    public string? Priority { get; set; }

    public int? Assigneduserid { get; set; }

    public DateTime? Createdat { get; set; }

    public virtual User? Assigneduser { get; set; }
}
