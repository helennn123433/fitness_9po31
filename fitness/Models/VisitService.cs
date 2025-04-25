using System;
using System.Collections.Generic;

namespace fitness.Models;

public partial class VisitService
{
    public int IdVisitServices { get; set; }

    public int IdVisit { get; set; }

    public int IdTime { get; set; }

    public int IdServices { get; set; }

    public virtual AdditionalService IdServicesNavigation { get; set; } = null!;

    public virtual Visit IdVisitNavigation { get; set; } = null!;
}
