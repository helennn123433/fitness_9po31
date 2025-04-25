using System;
using System.Collections.Generic;

namespace fitness.Models;

public partial class Hall
{
    public int IdHall { get; set; }

    public int CapacityHall { get; set; }

    public int IdTypeHall { get; set; }

    public int? Area { get; set; }

    public bool? IsActive { get; set; }

    public string? ImagePath { get; set; }

    public virtual HallType IdTypeHallNavigation { get; set; } = null!;

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
