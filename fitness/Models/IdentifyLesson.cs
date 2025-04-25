using System;
using System.Collections.Generic;

namespace fitness.Models;

public partial class IdentifyLesson
{
    public int IdIdentifyLesson { get; set; }

    public int IdAbonementClient { get; set; }

    public int IdTypeLesson { get; set; }

    public int IdTrainer { get; set; }

    public virtual AbonementClient IdAbonementClientNavigation { get; set; } = null!;

    public virtual Trainer IdTrainerNavigation { get; set; } = null!;

    public virtual LessonType IdTypeLessonNavigation { get; set; } = null!;
}
