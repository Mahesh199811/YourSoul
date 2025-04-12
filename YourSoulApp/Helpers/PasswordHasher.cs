using System;
using System.Security.Cryptography;
using System.Text;

namespace YourSoulApp.Helpers
{
    public static class PasswordHasher
    {
        // The size of the salt in bytes
        private const int SaltSize = 16;
        
        // The number of iterations for PBKDF2
        private const int Iterations = 10000;
        
        // The size of the hash in bytes
        private const int HashSize = 32;

        /// <summary>
        /// Hashes a password using PBKDF2 with a random salt
        /// </summary>
        /// <param name="password">The password to hash</param>
        /// <returns>A string in the format "iterations:salt:hash" where salt and hash are base64 encoded</returns>
        public static string HashPassword(string password)
        {
            // Generate a random salt
            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Hash the password with the salt
            byte[] hash = GetHash(password, salt, Iterations, HashSize);

            // Format: iterations:base64(salt):base64(hash)
            return $"{Iterations}:{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
        }

        /// <summary>
        /// Verifies a password against a stored hash
        /// </summary>
        /// <param name="password">The password to verify</param>
        /// <param name="hashedPassword">The stored hash in the format "iterations:salt:hash"</param>
        /// <returns>True if the password matches the hash, false otherwise</returns>
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            // Split the stored hash into its components
            string[] parts = hashedPassword.Split(':');
            if (parts.Length != 3)
                return false;

            // Parse the components
            if (!int.TryParse(parts[0], out int iterations))
                return false;

            byte[] salt;
            byte[] hash;
            
            try
            {
                salt = Convert.FromBase64String(parts[1]);
                hash = Convert.FromBase64String(parts[2]);
            }
            catch (FormatException)
            {
                return false;
            }

            // Hash the password with the same salt and iterations
            byte[] computedHash = GetHash(password, salt, iterations, hash.Length);

            // Compare the computed hash with the stored hash
            return SlowEquals(hash, computedHash);
        }

        /// <summary>
        /// Computes a hash using PBKDF2 with HMACSHA256
        /// </summary>
        private static byte[] GetHash(string password, byte[] salt, int iterations, int hashSize)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(hashSize);
            }
        }

        /// <summary>
        /// Compares two byte arrays in a way that protects against timing attacks
        /// </summary>
        private static bool SlowEquals(byte[] a, byte[] b)
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= (uint)(a[i] ^ b[i]);
            }
            return diff == 0;
        }
    }
}
