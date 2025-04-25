using System;
using System.Collections.Generic;

namespace fitness.Models;

public partial class User
{
    public int UserId { get; set; }

    public string NameUser { get; set; } = null!;

    public string LastnameUser { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public int? RoleId { get; set; }

    public int? AgeUser { get; set; }

    public string? Login { get; set; }

    public string? Password { get; set; }

    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();

    public virtual Role? Role { get; set; }

    public virtual ICollection<Trainer> Trainers { get; set; } = new List<Trainer>();
}
