using System;
using System.Collections.Generic;

namespace fitness.Models;

public partial class AdditionalService
{
    public int IdServices { get; set; }

    public string? ServicesName { get; set; }

    public int ServicesPrice { get; set; }

    public string ServicesDescription { get; set; } = null!;

    public string? ImagePath { get; set; }

    public virtual ICollection<VisitService> VisitServices { get; set; } = new List<VisitService>();
}
