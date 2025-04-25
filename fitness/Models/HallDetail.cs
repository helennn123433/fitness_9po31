using System;
using System.Collections.Generic;

namespace fitness.Models;

public partial class HallDetail
{
    public int? IdHall { get; set; }

    public int? Capacity { get; set; }

    public int? Area { get; set; }

    public bool? IsActive { get; set; }

    public string? HallType { get; set; }

    public string? TypeDescription { get; set; }
}
