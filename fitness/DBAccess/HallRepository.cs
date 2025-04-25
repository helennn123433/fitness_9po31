using fitness.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fitness.DBAccess
{
    public class HallRepository
    {
        public static List<HallDetail> GetAllHalls()
        {
            using (var context = new FitnessContext())
            {
                var halls = context.HallDetails.ToList();
                return halls;
            }
        }

        public static Hall GetHallById(int id) 
        {
            using (var context = new FitnessContext())
            {
                return context.Halls
                    .Include(h => h.IdTypeHallNavigation)
                    .FirstOrDefault(h => h.IdHall == id);
            }
        }
        public static List<HallDetail> GetActiveHalls()
        {
            using (var context = new FitnessContext())
            {
                var activeHalls = context.HallDetails.Where(h => h.IsActive == true).ToList();
                return activeHalls;
            }
        }

        public static List<HallType> GetHallTypes()
        {
            using (var context = new FitnessContext())
            {
                var halls = context.HallTypes.ToList();
                return halls;
            }
        }

        public static bool AddHall(Hall hall)
        {
            using (var context = new FitnessContext())
            {
                context.Halls.Add(hall);
                context.SaveChanges();
                return true;
            }
        }

        public static bool UpdateHall(Hall hall)
        {
            using (var context = new FitnessContext())
            {
                var existingHall = context.Halls
                    .FirstOrDefault(h => h.IdHall == hall.IdHall);

                if (existingHall == null)
                {
                    return false;
                }

                context.Entry(existingHall).CurrentValues.SetValues(hall);

                context.Entry(existingHall).State = EntityState.Modified;

                int affectedRows = context.SaveChanges();

                return affectedRows > 0;
            }
        }

        public static bool DeleteHall(int hallId)
        {
            using (var context = new FitnessContext())
            {
                var existingHall = context.Halls
                    .FirstOrDefault(h => h.IdHall == hallId);

                if (existingHall != null)
                {
                    context.Halls.Remove(existingHall);
                    context.SaveChanges();
                    return true;
                }

                return false;
            }
        }
    }
}
