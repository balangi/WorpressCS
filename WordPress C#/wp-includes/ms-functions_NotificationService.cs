using System.Net.Mail;

public class NotificationService
{
    private readonly ApplicationDbContext _context;

    public NotificationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public void NotifyAdminAboutNewSite(int siteId)
    {
        var adminEmail = _context.Settings.FirstOrDefault(s => s.Key == "AdminEmail")?.Value;
        if (string.IsNullOrEmpty(adminEmail))
            return;

        var site = _context.Sites.FirstOrDefault(s => s.Id == siteId);
        if (site == null)
            return;

        var message = $"A new site has been activated: {site.Name}";
        SendEmail(adminEmail, "New Site Notification", message);
    }

    public void NotifyAdminAboutNewUser(int userId)
    {
        var adminEmail = _context.Settings.FirstOrDefault(s => s.Key == "AdminEmail")?.Value;
        if (string.IsNullOrEmpty(adminEmail))
            return;

        var user = _context.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null)
            return;

        var message = $"A new user has been activated: {user.Username}";
        SendEmail(adminEmail, "New User Notification", message);
    }

    private void SendEmail(string to, string subject, string body)
    {
        using var smtpClient = new SmtpClient("smtp.example.com")
        {
            Port = 587,
            Credentials = new System.Net.NetworkCredential("username", "password"),
            EnableSsl = true
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress("noreply@example.com"),
            Subject = subject,
            Body = body,
            IsBodyHtml = false
        };
        mailMessage.To.Add(to);

        smtpClient.Send(mailMessage);
    }
}