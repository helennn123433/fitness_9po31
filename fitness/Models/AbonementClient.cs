using System;
using System.Collections.Generic;

namespace fitness.Models;

public partial class AbonementClient
{
    public int IdAbonementClient { get; set; }

    public DateOnly DateStart { get; set; }

    public DateOnly DateEnd { get; set; }

    public int IdClient { get; set; }

    public int IdAbonement { get; set; }

    public virtual Abonement IdAbonementNavigation { get; set; } = null!;

    public virtual Client IdClientNavigation { get; set; } = null!;

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

    public virtual ICollection<Visit> Visits { get; set; } = new List<Visit>();
}
