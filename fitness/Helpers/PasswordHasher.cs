using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace fitness.Helpers
{
    public class PasswordHasher
    {
        private static readonly byte[] FixedSalt = Convert.FromBase64String("AbCdEfGhIjKlMnOpQrStUvWxYz012345");
        public static string HashPassword(string password)
        {
            var pbkdf2 = new Rfc2898DeriveBytes(password, FixedSalt, 10000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(20);

            byte[] hashBytes = new byte[36];
            Array.Copy(FixedSalt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            return Convert.ToBase64String(hashBytes);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            byte[] hashBytes = Convert.FromBase64String(hashedPassword);

            var pbkdf2 = new Rfc2898DeriveBytes(password, FixedSalt, 10000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(20);

            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                    return false;
            }
            return true;
        }

    }
}
