using fitness.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fitness.DBAccess
{
    class AbonementRepository
    {
        public static List<Abonement> GetAllAbonement()
        {
            using (var context = new FitnessContext())
            {
                var abonements = context.Abonements.ToList();
                return abonements;
            }
        }

        public static bool AddAbonement(Abonement abonement)
        {
            using (var context = new FitnessContext())
            {
                context.Abonements.Add(abonement);
                context.SaveChanges();
                return true;
            }
        }

        public static bool UpdateAbonement(Abonement abonement)
        {
            using (var context = new FitnessContext())
            {
                var existingAbonement = context.Abonements
                    .FirstOrDefault(a => a.IdAbonement == abonement.IdAbonement);

                if (existingAbonement == null)
                {
                    return false;
                }

                context.Entry(existingAbonement).CurrentValues.SetValues(abonement);

                context.Entry(existingAbonement).State = EntityState.Modified;

                int affectedRows = context.SaveChanges();

                return affectedRows > 0;
            }
        }

        public static bool DeleteAbonement(Abonement abonement)
        {
            using (var context = new FitnessContext())
            {
                var existingAbonement = context.Abonements
                    .FirstOrDefault(a => a.IdAbonement == abonement.IdAbonement);

                if (existingAbonement != null)
                {
                    context.Abonements.Remove(existingAbonement);
                    context.SaveChanges();
                    return true;
                }

                return false;
            }
        }
    }
}
