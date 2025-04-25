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
    class TrainerRepository
    {
        public static List<TrainerDetail> GetAllTrainer()
        {
            using (var context = new FitnessContext())
            {
                var trainers = context.TrainerDetails.ToList();
                return trainers;
            }
        }

        public static bool AddTrainer(TrainerDetail trainer)
        {
            using (var context = new FitnessContext())
            {
                var result = context.BoolResults
                    .FromSqlInterpolated($@"
                        SELECT add_trainer(
                            {trainer.FirstName},
                            {trainer.LastName},
                            {trainer.MiddleName},
                            {trainer.Email},
                            {trainer.PhoneNumber},
                            {trainer.AgeUser},
                            {trainer.Login},
                            {trainer.Password},
                            {trainer.Education},
                            {trainer.ExperienceYears}
                        ) AS AddTrainer
                    ")
                    .AsEnumerable()
                    .FirstOrDefault();

                return result?.AddTrainer ?? false;
            }
        }

        public static bool UpdateTrainer(TrainerDetail trainer)
        {
            using (var context = new FitnessContext())
            {
                var result = context.BoolResults
                    .FromSqlInterpolated($@"
                    SELECT update_trainer(
                        {trainer.IdTrainer},
                        {trainer.FirstName},
                        {trainer.LastName},
                        {trainer.MiddleName},
                        {trainer.Email},
                        {trainer.PhoneNumber},
                        {trainer.AgeUser},
                        {trainer.Education},
                        {trainer.ExperienceYears}
                        ) AS AddTrainer
                    ")
                    .AsEnumerable()
                    .FirstOrDefault();

                return result?.AddTrainer ?? false;
            }
        }

        public static bool DeleteTrainer(TrainerDetail trainer)
        {
            using (var context = new FitnessContext())
            {
                var existingTrainer = context.Trainers
                    .FirstOrDefault(t => t.IdTrainer == trainer.IdTrainer);

                if (existingTrainer != null)
                {
                    context.Trainers.Remove(existingTrainer);
                    UserRepository.DeleteUser((int)existingTrainer.UserId);
                    int affectedRows = context.SaveChanges();
                    return affectedRows > 0;
                }

                return false;
            }
        }

        public static bool ResetPassword(TrainerDetail trainer)
        {
            using (var context = new FitnessContext())
            {
                var existingTrainer = context.Trainers
                    .FirstOrDefault(t => t.IdTrainer == trainer.IdTrainer);

                bool success = false;
                if (existingTrainer != null)
                {
                    success = UserRepository.ResetPassword((int)existingTrainer.UserId);
                    return success;
                }
                return false;

            }
        }

        public static List<TrainerDetail> GetAvailableTrainers(DateTime selectedDate, TimeOnly startTime, TimeOnly endTime, int? trainerId = null)
        {
            using (var context = new FitnessContext())
            {
                DateOnly selectedDateOnly = DateOnly.FromDateTime(selectedDate);
                var busyTrainerIds = context.Lessons
                    .Where(l => l.LessonDate == selectedDateOnly &&
                                l.StartTime < endTime &&
                                l.EndTime > startTime)
                    .Select(l => l.IdTrainer)
                    .Distinct()
                    .ToList();

                var trainers = context.TrainerDetails
                    .Where(t => !busyTrainerIds.Contains((int)t.IdTrainer) || t.IdTrainer == trainerId)
                    .ToList();

                return trainers;
            }
        }
    }
}
