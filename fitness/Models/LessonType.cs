using System;
using System.Collections.Generic;

namespace fitness.Models;

public partial class LessonType
{
    public int IdTypeLesson { get; set; }

    public string TypeLessonDesc { get; set; } = null!;

    public string TypeLessonName { get; set; } = null!;

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
