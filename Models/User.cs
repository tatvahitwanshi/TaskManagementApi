using System;
using System.Collections.Generic;

namespace TaskManagementApi.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Passwordhash { get; set; } = null!;

    public DateTime? Createdat { get; set; }

    public virtual ICollection<Task> Tasks { get; } = new List<Task>();

    public virtual ICollection<Userrole> Userroles { get; } = new List<Userrole>();
}
