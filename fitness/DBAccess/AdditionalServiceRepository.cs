using fitness.Helpers;
using fitness.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fitness.DBAccess
{
    public class AdditionalServiceRepository
    {
        public static List<AdditionalService> GetAllAdditionalService()
        {
            using (var context = new FitnessContext())
            {
                var additionalService = context.AdditionalServices.ToList();
                return additionalService;
            }
        }

        public static bool AddAdditionalService(AdditionalService additionalService)
        {
            using (var context = new FitnessContext())
            {
                context.AdditionalServices.Add(additionalService);
                context.SaveChanges();
                return true;
            }
        }

        public static bool UpdateAdditionalService(AdditionalService additionalService)
        {
            using (var context = new FitnessContext())
            {
                var existingService = context.AdditionalServices
                    .FirstOrDefault(s => s.IdServices == additionalService.IdServices);

                if (existingService == null)
                {
                    return false;
                }

                context.Entry(existingService).CurrentValues.SetValues(additionalService);

                context.Entry(existingService).State = EntityState.Modified;

                int affectedRows = context.SaveChanges();

                return affectedRows > 0;
            }
        }

        public static bool DeleteAdditionalService(AdditionalService additionalService)
        {
            using (var context = new FitnessContext())
            {
                var existingService = context.AdditionalServices
                    .FirstOrDefault(s => s.IdServices == additionalService.IdServices);

                if (existingService != null)
                {
                    context.AdditionalServices.Remove(existingService);
                    context.SaveChanges();
                    return true;
                }

                return false;
            }
        }
    }
}
