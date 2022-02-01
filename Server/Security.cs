using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VacationManagerLibrary;

namespace VacationManagerServer
{
    public static class Security
    {
        public static byte[] HashPassword(byte[] password, ref byte[]? salt)
        {
            password = Hashing(password);
            if (salt == null)
            {
                password = Salting(password, out salt);
                return Hashing(password);
            }
            else
            {
                byte[] result = new byte[salt.Length + password.Length];
                salt.CopyTo(result, 0);
                password.CopyTo(result, salt.Length);
                return Hashing(result);
            }
        }
        private static byte[] Hashing(byte[] password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(password);
                return hash;
            }
        }
        private static byte[] Salting(byte[] password, out byte[] salt)
        {
            salt = new byte[8];
            Service.rng.NextBytes(salt);
            byte[] result = new byte[salt.Length + password.Length];
            salt.CopyTo(result, 0);
            password.CopyTo(result, salt.Length);
            return result;
        }
    }
}
