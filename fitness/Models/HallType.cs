using System;
using System.Collections.Generic;

namespace fitness.Models;

public partial class HallType
{
    public int IdTypeHall { get; set; }

    public string NameTypeHall { get; set; } = null!;

    public string TypeHallDesc { get; set; } = null!;

    public virtual ICollection<Hall> Halls { get; set; } = new List<Hall>();
}
