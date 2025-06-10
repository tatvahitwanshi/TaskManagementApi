using System;
using System.Collections.Generic;

namespace TaskManagementApi.Models;

public partial class Role
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Userrole> Userroles { get; } = new List<Userrole>();
}
