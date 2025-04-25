using System;
using System.Collections.Generic;

namespace fitness.Models;

public partial class Visit
{
    public int IdVisit { get; set; }

    public DateOnly DateVisit { get; set; }

    public int DateTime { get; set; }

    public int IdAbonementClient { get; set; }

    public virtual AbonementClient IdAbonementClientNavigation { get; set; } = null!;

    public virtual ICollection<VisitService> VisitServices { get; set; } = new List<VisitService>();
}
