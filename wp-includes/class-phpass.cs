using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Phpass
{
    /// <summary>
    /// Represents a portable password hashing framework.
    /// </summary>
    public class PasswordHash
    {
        private readonly string _itoa64 = "./0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private readonly int _iterationCountLog2;
        private readonly bool _portableHashes;
        private string _randomState;

        /// <summary>
        /// Constructor to initialize the PasswordHash object.
        /// </summary>
        /// <param name="iterationCountLog2">The iteration count in log2 format (between 4 and 31).</param>
        /// <param name="portableHashes">Whether to use portable hashes.</param>
        public PasswordHash(int iterationCountLog2, bool portableHashes)
        {
            if (iterationCountLog2 < 4 || iterationCountLog2 > 31)
                iterationCountLog2 = 8;

            _iterationCountLog2 = iterationCountLog2;
            _portableHashes = portableHashes;
            _randomState = DateTime.Now.Ticks.ToString();
        }

        /// <summary>
        /// Generates random bytes for cryptographic purposes.
        /// </summary>
        /// <param name="count">The number of random bytes to generate.</param>
        /// <returns>A byte array containing random bytes.</returns>
        private byte[] GetRandomBytes(int count)
        {
            var output = new byte[count];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(output);
            }
            return output;
        }

        /// <summary>
        /// Encodes binary data into a base64-like string.
        /// </summary>
        /// <param name="input">The input byte array.</param>
        /// <param name="count">The number of bytes to encode.</param>
        /// <returns>An encoded string.</returns>
        private string Encode64(byte[] input, int count)
        {
            var output = new StringBuilder();
            int i = 0;

            while (i < count)
            {
                int value = input[i++];
                output.Append(_itoa64[value & 0x3f]);

                if (i < count)
                    value |= input[i] << 8;

                output.Append(_itoa64[(value >> 6) & 0x3f]);

                if (i++ >= count)
                    break;

                if (i < count)
                    value |= input[i] << 16;

                output.Append(_itoa64[(value >> 12) & 0x3f]);

                if (i++ >= count)
                    break;

                output.Append(_itoa64[(value >> 18) & 0x3f]);
            }

            return output.ToString();
        }

        /// <summary>
        /// Generates a salt for private hashing.
        /// </summary>
        /// <param name="input">The input byte array.</param>
        /// <returns>A salt string.</returns>
        private string GenerateSaltPrivate(byte[] input)
        {
            var output = "$P$";
            output += _itoa64[Math.Min(_iterationCountLog2 + 5, 30)];
            output += Encode64(input, 6);
            return output;
        }

        /// <summary>
        /// Hashes a password using a private algorithm.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <param name="setting">The salt or existing hash settings.</param>
        /// <returns>The hashed password.</returns>
        private string CryptPrivate(string password, string setting)
        {
            if (string.IsNullOrEmpty(setting) || setting.Length < 12)
                return "*";

            var id = setting.Substring(0, 3);
            if (id != "$P$" && id != "$H$")
                return "*";

            var countLog2 = _itoa64.IndexOf(setting[3]);
            if (countLog2 < 7 || countLog2 > 30)
                return "*";

            var count = 1 << countLog2;
            var salt = setting.Substring(4, 8);

            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(salt + password));
                for (int i = 0; i < count; i++)
                {
                    hash = md5.ComputeHash(hash.Concat(Encoding.UTF8.GetBytes(password)).ToArray());
                }

                var output = setting.Substring(0, 12);
                output += Encode64(hash, 16);
                return output;
            }
        }

        /// <summary>
        /// Hashes a password using bcrypt.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <returns>The hashed password.</returns>
        public string HashPassword(string password)
        {
            if (password.Length > 4096)
                return "*";

            var randomBytes = GetRandomBytes(16);
            var salt = GenerateSaltPrivate(randomBytes);

            var hash = CryptPrivate(password, salt);
            return hash;
        }

        /// <summary>
        /// Verifies a password against a stored hash.
        /// </summary>
        /// <param name="password">The password to verify.</param>
        /// <param name="storedHash">The stored hash.</param>
        /// <returns>True if the password matches the hash; otherwise, false.</returns>
        public bool CheckPassword(string password, string storedHash)
        {
            if (password.Length > 4096)
                return false;

            var hash = CryptPrivate(password, storedHash);
            return hash == storedHash;
        }
    }
}