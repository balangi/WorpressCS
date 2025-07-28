using System;
using System.Collections.Generic;

public class SessionToken
{
    public int Id { get; set; }
    public string Token { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
}

public class User
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public List<SessionToken> SessionTokens { get; set; } = new List<SessionToken>();
}