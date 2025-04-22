using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using PHPMailer.Core.Data;
using PHPMailer.Core.Models;

namespace PHPMailer.Core
{
    public class PHPMailer
    {
        // Properties
        public string Host { get; set; } = "smtp.example.com";
        public int Port { get; set; } = 587;
        public string Username { get; set; }
        public string Password { get; set; }
        public bool EnableSsl { get; set; } = true;
        public string FromAddress { get; set; }
        public string FromName { get; set; }
        public List<string> ToAddresses { get; set; } = new List<string>();
        public List<string> CcAddresses { get; set; } = new List<string>();
        public List<string> BccAddresses { get; set; } = new List<string>();
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsHtml { get; set; } = true;

        private readonly AppDbContext _dbContext;

        public PHPMailer(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Sends the email using SMTP.
        /// </summary>
        /// <returns>True if the email was sent successfully; otherwise, false.</returns>
        public async Task<bool> SendAsync()
        {
            try
            {
                using (var smtpClient = new SmtpClient(Host, Port))
                {
                    smtpClient.Credentials = new NetworkCredential(Username, Password);
                    smtpClient.EnableSsl = EnableSsl;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(FromAddress, FromName),
                        Subject = Subject,
                        Body = Body,
                        IsBodyHtml = IsHtml
                    };

                    foreach (var to in ToAddresses)
                        mailMessage.To.Add(to);

                    foreach (var cc in CcAddresses)
                        mailMessage.CC.Add(cc);

                    foreach (var bcc in BccAddresses)
                        mailMessage.Bcc.Add(bcc);

                    await smtpClient.SendMailAsync(mailMessage);

                    // Log the email
                    _dbContext.EmailLogs.Add(new EmailLog
                    {
                        ToAddress = string.Join(", ", ToAddresses),
                        Subject = Subject,
                        Body = Body,
                        SentAt = DateTime.UtcNow,
                        IsSuccessful = true
                    });
                    await _dbContext.SaveChangesAsync();

                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log the error
                _dbContext.EmailLogs.Add(new EmailLog
                {
                    ToAddress = string.Join(", ", ToAddresses),
                    Subject = Subject,
                    Body = Body,
                    SentAt = DateTime.UtcNow,
                    IsSuccessful = false,
                    ErrorMessage = ex.Message
                });
                await _dbContext.SaveChangesAsync();

                throw new Exception("Failed to send email.", ex);
            }
        }

        /// <summary>
        /// Validates an email address.
        /// </summary>
        /// <param name="email">The email address to validate.</param>
        /// <returns>True if the email is valid; otherwise, false.</returns>
        public static bool ValidateAddress(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}