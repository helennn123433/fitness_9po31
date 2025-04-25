using System;
using System.Collections.Generic;

namespace fitness.Models;

public partial class UserWithRole
{
    public int? UserId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? MiddleName { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public int? AgeUser { get; set; }

    public string? Login { get; set; }

    public string? RoleName { get; set; }
}
