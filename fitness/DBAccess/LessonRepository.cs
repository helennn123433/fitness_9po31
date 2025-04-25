using fitness.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace fitness.DBAccess
{
    public class LessonRepository
    {
        public static List<LessonScheduleView> GetAllLessons()
        {
            using (var context = new FitnessContext())
            {
                var lessons = context.LessonScheduleViews.ToList();
                return lessons;
            }
        }

        public static List<LessonScheduleView> GetLessonsForWeek(DateTime startOfWeek, DateTime endOfWeek, string? lessonTypeName = null, string? trainerFullName = null)
        {
            using (var context = new FitnessContext())
            {
                DateOnly startDate = DateOnly.FromDateTime(startOfWeek);
                DateOnly endDate = DateOnly.FromDateTime(endOfWeek);

                var query = context.LessonScheduleViews
                    .Where(l => l.LessonDate >= startDate && l.LessonDate <= endDate);

                if (lessonTypeName != null)
                    query = query.Where(l => l.LessonTypeName == lessonTypeName);

                if (trainerFullName != null)
                    query = query.Where(l => l.TrainerFullName == trainerFullName);

                return query.ToList();
            }
        }

        public static Lesson GetLessonById(int? id)
        {
            using (var context = new FitnessContext())
            {
                var lesson = context.Lessons.FirstOrDefault(l => l.IdLesson == id);
                return lesson;
            }
        }

        public static List<Lesson> GetLessonsByHallAndDate(int idHall, DateTime selectedDate)
        {
            using (var context = new FitnessContext())
            {
                DateOnly selectedDateOnly = DateOnly.FromDateTime(selectedDate);
                var lessons = context.Lessons
                    .Where(l => l.IdHall == idHall && l.LessonDate == selectedDateOnly)
                    .ToList();
                return lessons;
            }
        }

        public static List<LessonType> GetAllLessonsType()
        {
            using (var context = new FitnessContext())
            {
                var lessonTypes = context.LessonTypes.ToList();
                return lessonTypes;
            }
        }

        public static bool AddLesson(Lesson lesson) 
        {
            using (var context = new FitnessContext())
            {
                context.Lessons.Add(lesson);
                return context.SaveChanges() > 0;
            }
        }

        public static bool UpdateLesson(Lesson lesson)
        {
            using (var context = new FitnessContext())
            {
                context.Lessons.Update(lesson);
                return context.SaveChanges() > 0;
            }
        }
        
        public static bool DeleteLesson(int lessonId)
        {
            using (var context = new FitnessContext())
            {
                var lesson = context.Lessons.FirstOrDefault(l => l.IdLesson == lessonId);

                context.Lessons.Remove(lesson);
                return context.SaveChanges() > 0;
            }
        }
    }
}
