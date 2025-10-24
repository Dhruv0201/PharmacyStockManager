using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace PharmacyStockManager.Helpers
{
    static class PasswordHashHelper
    {
        public static (string HashedPassword, string Salt) HashPasswordWithSalt(string password)
        {
            const int saltSize = 16;      // 16 bytes -> Base64 length 24
            const int keySize = 32;       // 32 bytes -> Base64 length 44
            const int iterations = 100_000;

            var saltBytes = RandomNumberGenerator.GetBytes(saltSize);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, iterations, HashAlgorithmName.SHA256);
            var keyBytes = pbkdf2.GetBytes(keySize);

            var hashed = Convert.ToBase64String(keyBytes);
            var salt = Convert.ToBase64String(saltBytes);

            return (HashedPassword: hashed, Salt: salt);
        }

      
        public static bool VerifyPassword(string password, string storedHashBase64, string storedSaltBase64)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedHashBase64) || string.IsNullOrEmpty(storedSaltBase64))
                return false;

            var saltBytes = Convert.FromBase64String(storedSaltBase64);
            const int iterations = 100_000;
            const int keySize = 32;

            using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, iterations, HashAlgorithmName.SHA256);
            var computedKey = pbkdf2.GetBytes(keySize);
            var storedKey = Convert.FromBase64String(storedHashBase64);

            return CryptographicOperations.FixedTimeEquals(computedKey, storedKey);
        }

        public static string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}