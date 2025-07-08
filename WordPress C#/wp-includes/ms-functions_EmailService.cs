public class EmailService
{
    public void SendWelcomeEmail(string to, string username, string activationUrl)
    {
        var subject = "Welcome to Our Network!";
        var body = $"Hello {username},\n\nPlease activate your account by clicking the following link: {activationUrl}";

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