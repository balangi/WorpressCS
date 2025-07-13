using System;
using System.Collections.Generic;

public class EmailLog
{
    public int Id { get; set; }
    public string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public string Headers { get; set; }
    public List<string> Attachments { get; set; } = new List<string>();
    public DateTime SentAt { get; set; }
    public bool IsSuccessful { get; set; }
    public string ErrorMessage { get; set; }
}

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public bool IsActive { get; set; }
}