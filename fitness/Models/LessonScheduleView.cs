using System;
using System.Collections.Generic;

namespace fitness.Models;

public partial class LessonScheduleView
{
    public int? IdLesson { get; set; }

    public string? TypeHallName { get; set; }

    public DateOnly? LessonDate { get; set; }

    public TimeOnly? LessonStart { get; set; }

    public TimeOnly? LessonEnd { get; set; }

    public string? TrainerFullName { get; set; }

    public int? IdAbonement { get; set; }

    public string? LessonTypeName { get; set; }
}
