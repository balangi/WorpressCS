using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class PluggableService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PluggableService> _logger;

    public PluggableService(ApplicationDbContext context, ILogger<PluggableService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Sends an email using the provided parameters.
    /// </summary>
    public bool SendEmail(string to, string subject, string body, string headers = null, List<string> attachments = null)
    {
        try
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress("noreply@example.com"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(to);

            if (attachments != null)
            {
                foreach (var attachment in attachments)
                {
                    mailMessage.Attachments.Add(new Attachment(attachment));
                }
            }

            using (var smtpClient = new SmtpClient("smtp.example.com"))
            {
                smtpClient.Send(mailMessage);
            }

            LogEmail(to, subject, body, headers, attachments, true, null);
            return true;
        }
        catch (Exception ex)
        {
            LogEmail(to, subject, body, headers, attachments, false, ex.Message);
            _logger.LogError(ex, "Failed to send email.");
            return false;
        }
    }

    /// <summary>
    /// Logs email details into the database.
    /// </summary>
    private void LogEmail(string to, string subject, string body, string headers, List<string> attachments, bool isSuccessful, string errorMessage)
    {
        var log = new EmailLog
        {
            To = to,
            Subject = subject,
            Body = body,
            Headers = headers,
            Attachments = attachments ?? new List<string>(),
            SentAt = DateTime.UtcNow,
            IsSuccessful = isSuccessful,
            ErrorMessage = errorMessage
        };

        _context.EmailLogs.Add(log);
        _context.SaveChanges();
    }

    /// <summary>
    /// Authenticates a user by username and password.
    /// </summary>
    public bool AuthenticateUser(string username, string password)
    {
        var user = _context.Users.FirstOrDefault(u => u.Username == username && u.IsActive);
        if (user == null)
        {
            return false;
        }

        // Simulate password hash comparison
        return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    public bool CreateUser(string username, string email, string password)
    {
        if (_context.Users.Any(u => u.Username == username || u.Email == email))
        {
            throw new InvalidOperationException("Username or email already exists.");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = passwordHash,
            IsActive = true
        };

        _context.Users.Add(user);
        _context.SaveChanges();
        return true;
    }
}