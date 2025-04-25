using System;
using System.Collections.Generic;

namespace fitness.Models;

public partial class ClientInfoView
{
    public int? ClientId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? MiddleName { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public int? Age { get; set; }

    public string? AbonementName { get; set; }

    public int? IdAbonementClient { get; set; }

    public DateOnly? AbonementStartDate { get; set; }

    public DateOnly? AbonementEndDate { get; set; }

    public string? AbonementStatus { get; set; }

    public int? DaysRemaining { get; set; }

    public string? Login { get; set; }

    public string? Password { get; set; }
}
