using fitness.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fitness.DBAccess
{
    public class ClientRepository
    {
        public static List<ClientInfoView> GetAllClients()
        {
            using (var context = new FitnessContext())
            {
                return context.ClientInfoViews.ToList();
            }
        }
        public static List<ClientInfoView> GetAvailableClients(DateTime selectedDate, TimeOnly startTime, TimeOnly endTime, int? idAbonement = null)
        {
            using (var context = new FitnessContext())
            {
                DateOnly selectedDateOnly = DateOnly.FromDateTime(selectedDate);
                var busyClientIds = context.Lessons
                    .Where(l => l.LessonDate == selectedDateOnly &&
                                l.StartTime < endTime &&
                                l.EndTime > startTime &&
                                l.IdAbonementClient != null)
                    .Select(l => l.IdAbonementClient.Value)
                    .Distinct()
                    .ToList();

                var clients = context.ClientInfoViews
                    .Where(c => (!busyClientIds.Contains((int)c.IdAbonementClient) || c.IdAbonementClient == idAbonement) && c.DaysRemaining > 0 )
                    .ToList();

                return clients;
            }
        }

        public static bool AddClient(ClientInfoView client)
        {
            bool isSuccess = false;

            using (var context = new FitnessContext())
            {
                var connection = (NpgsqlConnection)context.Database.GetDbConnection();
                connection.Open();

                using (var command = new NpgsqlCommand("CALL public.register_client_with_abonement(@p_first_name, @p_last_name, @p_middle_name, @p_email, @p_phone, @p_age, @p_login, @p_password, @p_abonement_name, @p_start_date, true)", connection))
                {
                    command.Parameters.Add("p_first_name", NpgsqlTypes.NpgsqlDbType.Varchar).Value = client.FirstName ?? "";
                    command.Parameters.Add("p_last_name", NpgsqlTypes.NpgsqlDbType.Varchar).Value = client.LastName ?? "";
                    command.Parameters.Add("p_middle_name", NpgsqlTypes.NpgsqlDbType.Varchar).Value = client.MiddleName ?? "";
                    command.Parameters.Add("p_email", NpgsqlTypes.NpgsqlDbType.Varchar).Value = client.Email ?? "";
                    command.Parameters.Add("p_phone", NpgsqlTypes.NpgsqlDbType.Varchar).Value = client.PhoneNumber ?? "";
                    command.Parameters.Add("p_age", NpgsqlTypes.NpgsqlDbType.Integer).Value = client.Age ?? 0;
                    command.Parameters.Add("p_login", NpgsqlTypes.NpgsqlDbType.Varchar).Value = client.Login ?? "";
                    command.Parameters.Add("p_password", NpgsqlTypes.NpgsqlDbType.Varchar).Value = client.Password ?? "";
                    command.Parameters.Add("p_abonement_name", NpgsqlTypes.NpgsqlDbType.Varchar).Value = client.AbonementName ?? "";
                    command.Parameters.Add("p_start_date", NpgsqlTypes.NpgsqlDbType.Date).Value =
                        client.AbonementStartDate.HasValue
                            ? client.AbonementStartDate
                            : DateTime.Now.Date;
                    command.ExecuteNonQuery();
                    isSuccess = true;
                }
            }

            return isSuccess;
        }

        public static bool UpdateClient(ClientInfoView client)
        {
            bool isSuccess = false;

            using (var context = new FitnessContext())
            {
                var connection = (NpgsqlConnection)context.Database.GetDbConnection();
                connection.Open();

                using (var command = new NpgsqlCommand("CALL public.update_client_with_abonement(@p_login, @p_first_name, @p_last_name, @p_middle_name, @p_email, @p_phone, @p_age, @p_abonement_name, @p_start_date, false)", connection))
                {
                    command.Parameters.AddWithValue("p_login", client.Login ?? "");
                    command.Parameters.Add("p_first_name", NpgsqlTypes.NpgsqlDbType.Varchar).Value = client.FirstName ?? "";
                    command.Parameters.Add("p_last_name", NpgsqlTypes.NpgsqlDbType.Varchar).Value = client.LastName ?? "";
                    command.Parameters.Add("p_middle_name", NpgsqlTypes.NpgsqlDbType.Varchar).Value = client.MiddleName ?? "";
                    command.Parameters.Add("p_email", NpgsqlTypes.NpgsqlDbType.Varchar).Value = client.Email ?? "";
                    command.Parameters.Add("p_phone", NpgsqlTypes.NpgsqlDbType.Varchar).Value = client.PhoneNumber ?? "";
                    command.Parameters.Add("p_age", NpgsqlTypes.NpgsqlDbType.Integer).Value = client.Age ?? 0;
                    command.Parameters.Add("p_abonement_name", NpgsqlTypes.NpgsqlDbType.Varchar).Value = client.AbonementName ?? "";
                    command.Parameters.Add("p_start_date", NpgsqlTypes.NpgsqlDbType.Date).Value =
                        client.AbonementStartDate.HasValue
                            ? client.AbonementStartDate
                            : DateTime.Now.Date;
                    command.ExecuteNonQuery();

                    isSuccess = true;
                }
            }
            return isSuccess;
        }

        public static bool DeleteClient(ClientInfoView client)
        {
            using (var context = new FitnessContext())
            {
                var selectedClient = context.Clients.FirstOrDefault(c => c.IdClient == client.ClientId);
                return UserRepository.DeleteUser((int)selectedClient.UserId);
            }
        }

        public static bool ResetPassword(int clientId)
        {
            using (var context = new FitnessContext())
            {
                var selectedUser = context.Clients.FirstOrDefault(c => c.IdClient == clientId);
                return UserRepository.ResetPassword((int)selectedUser.UserId);
            }
        }
    }
}
