using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WordPress.Core.Data;
using WordPress.Core.Models;

namespace WordPress.Core.ApplicationPasswords
{
    public class ApplicationPasswordsManager
    {
        private readonly AppDbContext _dbContext;

        public const string USERMETA_KEY_APPLICATION_PASSWORDS = "_application_passwords";
        public const string OPTION_KEY_IN_USE = "using_application_passwords";
        public const int PW_LENGTH = 24;

        public ApplicationPasswordsManager(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Checks if application passwords are being used by the site.
        /// </summary>
        public bool IsInUse()
        {
            return _dbContext.Users.Any(u => u.ApplicationPasswords.Any());
        }

        /// <summary>
        /// Creates a new application password.
        /// </summary>
        public (string PlainTextPassword, ApplicationPassword Details) CreateNewApplicationPassword(int userId, string name, string appId = "")
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("An application name is required to create an application password.");
            }

            var plainTextPassword = GenerateRandomPassword(PW_LENGTH);
            var hashedPassword = HashPassword(plainTextPassword);

            var newPassword = new ApplicationPassword
            {
                Uuid = Guid.NewGuid().ToString(),
                AppId = appId,
                Name = name,
                PasswordHash = hashedPassword,
                Created = DateTime.UtcNow,
                LastUsed = null,
                LastIp = null
            };

            var user = _dbContext.Users.Include(u => u.ApplicationPasswords).FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            user.ApplicationPasswords.Add(newPassword);
            _dbContext.SaveChanges();

            return (plainTextPassword, newPassword);
        }

        /// <summary>
        /// Gets all application passwords for a user.
        /// </summary>
        public List<ApplicationPassword> GetUserApplicationPasswords(int userId)
        {
            var user = _dbContext.Users.Include(u => u.ApplicationPasswords).FirstOrDefault(u => u.Id == userId);
            return user?.ApplicationPasswords ?? new List<ApplicationPassword>();
        }

        /// <summary>
        /// Gets a specific application password by UUID.
        /// </summary>
        public ApplicationPassword GetApplicationPassword(int userId, string uuid)
        {
            var user = _dbContext.Users.Include(u => u.ApplicationPasswords).FirstOrDefault(u => u.Id == userId);
            return user?.ApplicationPasswords.FirstOrDefault(p => p.Uuid == uuid);
        }

        /// <summary>
        /// Updates an application password.
        /// </summary>
        public bool UpdateApplicationPassword(int userId, string uuid, string newName)
        {
            var password = GetApplicationPassword(userId, uuid);
            if (password == null)
            {
                throw new InvalidOperationException("Application password not found.");
            }

            password.Name = newName;
            _dbContext.SaveChanges();
            return true;
        }

        /// <summary>
        /// Deletes an application password.
        /// </summary>
        public bool DeleteApplicationPassword(int userId, string uuid)
        {
            var user = _dbContext.Users.Include(u => u.ApplicationPasswords).FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            var password = user.ApplicationPasswords.FirstOrDefault(p => p.Uuid == uuid);
            if (password == null)
            {
                throw new InvalidOperationException("Application password not found.");
            }

            user.ApplicationPasswords.Remove(password);
            _dbContext.SaveChanges();
            return true;
        }

        /// <summary>
        /// Records usage of an application password.
        /// </summary>
        public bool RecordUsage(int userId, string uuid, string ipAddress)
        {
            var password = GetApplicationPassword(userId, uuid);
            if (password == null)
            {
                throw new InvalidOperationException("Application password not found.");
            }

            password.LastUsed = DateTime.UtcNow;
            password.LastIp = ipAddress;
            _dbContext.SaveChanges();
            return true;
        }

        /// <summary>
        /// Generates a random password.
        /// </summary>
        private string GenerateRandomPassword(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Hashes a plaintext password.
        /// </summary>
        private string HashPassword(string password)
        {
            // Use a secure hashing algorithm like bcrypt or PBKDF2
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// Verifies a password against its hash.
        /// </summary>
        public bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}