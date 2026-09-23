// Thiago Augusto Ruskowski Waltrick
using System;
using System.Security.Cryptography;
using Konscious.Security.Cryptography;
using System.Text;

namespace AcademiaDoZe.Application.Security
{
    public static class PasswordHasher
    {
        // Parâmetros conforme material de referência do projeto
        private const int SaltSize = 16; // bytes
        private const int HashSize = 32; // bytes
        private const int Iterations = 3; // time cost
        private const int MemoryKB = 65536; // memory cost em KB (64 MB)
        private const int DegreeOfParallelism = 2; // lanes / paralelismo

        public static string Hash(string password)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));

            var salt = new byte[SaltSize];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);

            var hash = Argon2idHash(Encoding.UTF8.GetBytes(password), salt);

            // formato: iterations.memory.parallel.salt.hash (base64)
            var parts = new string[] {
                Iterations.ToString(),
                MemoryKB.ToString(),
                DegreeOfParallelism.ToString(),
                Convert.ToBase64String(salt),
                Convert.ToBase64String(hash)
            };

            return string.Join('.', parts);
        }

        public static bool Verify(string password, string encodedHash)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (encodedHash == null) throw new ArgumentNullException(nameof(encodedHash));

            var parts = encodedHash.Split('.', 5);
            if (parts.Length != 5) return false;

            if (!int.TryParse(parts[0], out var iterations)) return false;
            if (!int.TryParse(parts[1], out var memoryKb)) return false;
            if (!int.TryParse(parts[2], out var parallelism)) return false;

            var salt = Convert.FromBase64String(parts[3]);
            var hash = Convert.FromBase64String(parts[4]);

            var computed = Argon2idHash(Encoding.UTF8.GetBytes(password), salt, iterations, memoryKb, parallelism, hash.Length);

            return CryptographicOperations.FixedTimeEquals(hash, computed);
        }

        private static byte[] Argon2idHash(byte[] passwordBytes, byte[] salt, int iterations = Iterations, int memoryKb = MemoryKB, int parallelism = DegreeOfParallelism, int hashLength = HashSize)
        {
            var argon = new Argon2id(passwordBytes)
            {
                Salt = salt,
                DegreeOfParallelism = parallelism,
                Iterations = iterations,
                MemorySize = memoryKb
            };

            return argon.GetBytes(hashLength);
        }
    }
}
