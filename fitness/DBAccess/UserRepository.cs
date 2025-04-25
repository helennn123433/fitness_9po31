using fitness.Models;
using fitness.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;

namespace fitness.DBAccess
{
    public class UserRepository
    {
        public static User Authenticate(string login, string password)
        {
            using (var context = new FitnessContext())
            {
                var user = context.Users
                    .Include(u => u.Role)
                    .FirstOrDefault(u => u.Login == login);
                if(user != null)
                {
                    if (IsCorrectPassword(user.UserId, password))
                        return user;
                }
                return null;
            }
        }

        public static List<UserWithRole> GetAllManagers()
        {
            using (var context = new FitnessContext())
            {
                var managers = context.UserWithRoles
                    .Where(u => u.RoleName == "менеджер")
                    .ToList();

                return managers;
            }
        }

        public static bool ResetPassword(int userId)
        {
            using (var context = new FitnessContext())
            {
                var existingUser = context.Users
                    .FirstOrDefault(u => u.UserId == userId);

                existingUser.Password = PasswordHasher.HashPassword("");
                context.SaveChanges();

                return true;
            }
        }

        public static bool AddUser(User user, int roleId)
        {
            using (var context = new FitnessContext())
            {
                user.RoleId = roleId;
                context.Users.Add(user);
                int affectedRows = context.SaveChanges();
                return affectedRows > 0;
            }
        }

        public static bool AddManager(User user)
        {
            using (var context = new FitnessContext())
            {
                var role = context.Roles
                    .FirstOrDefault(r => r.Name == "менеджер");

                return AddUser(user, role.Id); ;
            }
        }

        public static bool UpdateUser(User user)
        {
            using (var context = new FitnessContext())
            {
                var existingUser = context.Users.FirstOrDefault(u => u.UserId == user.UserId);

                if (existingUser != null)
                {
                    existingUser.NameUser = user.NameUser;
                    existingUser.LastnameUser = user.LastnameUser;
                    existingUser.MiddleName = user.MiddleName;
                    existingUser.Email = user.Email;
                    existingUser.PhoneNumber = user.PhoneNumber;
                    existingUser.AgeUser = user.AgeUser;

                    int affectedRows = context.SaveChanges();

                    return affectedRows > 0;
                }
                return false;
            }
        }

        public static bool DeleteUser(int userId)
        {
            using (var context = new FitnessContext())
            {
                var user = context.Users.FirstOrDefault(u => userId == u.UserId);
                context.Users.Remove(user);
                int affectedRows = context.SaveChanges();

                return affectedRows > 0;
            }
        }

        public static bool IsCorrectPassword(int userId, string password)
        {
            using (var context = new FitnessContext())
            {
                var user = context.Users.FirstOrDefault(u => u.UserId == userId);
                if (user != null)
                {
                    if (user.Password == PasswordHasher.HashPassword(password))
                        return true;
                }
                return false;
            }
        }

        public static void UpdatePassword(int userId, string oldPassword, string newPassword)
        {
            using (var context = new FitnessContext())
            {
                var user = context.Users.FirstOrDefault(u => u.UserId == userId);
                if (user != null)
                {
                    if (user.Password == PasswordHasher.HashPassword(oldPassword))
                    {
                        user.Password = PasswordHasher.HashPassword(newPassword);
                        context.SaveChanges();
                    }
                }
            }
        }

        public static bool IsAdmin(int userId)
        {
            using (var context = new FitnessContext())
            {
                var user = context.UserWithRoles.FirstOrDefault(u => u.UserId == userId);
                if (user != null)
                {
                    return user.RoleName == "админ";
                }
                return false;
            }
        }

        public static bool IsManager(int userId)
        {
            using (var context = new FitnessContext())
            {
                var user = context.UserWithRoles.FirstOrDefault(u => u.UserId == userId);
                if (user != null)
                {
                    return user.RoleName == "менеджер";
                }
                return false;
            }
        }

        public static bool IsTrainer(int userId)
        {
            using (var context = new FitnessContext())
            {
                var user = context.UserWithRoles.FirstOrDefault(u => u.UserId == userId);
                if (user != null)
                {
                    return user.RoleName == "тренер";
                }
                return false;
            }
        }
    }
}
