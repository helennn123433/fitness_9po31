using System;
using System.Collections.Generic;

namespace fitness.Models;

public partial class Lesson
{
    public int IdLesson { get; set; }

    public int IdHall { get; set; }

    public DateOnly LessonDate { get; set; }

    public int IdTrainer { get; set; }

    public int IdTypeLesson { get; set; }

    public int? IdAbonementClient { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public virtual AbonementClient? IdAbonementClientNavigation { get; set; }

    public virtual Hall IdHallNavigation { get; set; } = null!;

    public virtual Trainer IdTrainerNavigation { get; set; } = null!;

    public virtual LessonType IdTypeLessonNavigation { get; set; } = null!;
}
