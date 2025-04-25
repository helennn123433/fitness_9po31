using System;
using System.Collections.Generic;

namespace fitness.Models;

public partial class Trainer
{
    public int IdTrainer { get; set; }

    public string TrainerEducation { get; set; } = null!;

    public int TrainerExperiance { get; set; }

    public int? UserId { get; set; }

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

    public virtual User? User { get; set; }
}
