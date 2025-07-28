using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class SessionService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SessionService> _logger;

    public SessionService(ApplicationDbContext context, ILogger<SessionService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new session token for a user.
    /// </summary>
    public SessionToken CreateSessionToken(string userId, DateTime expiryDate)
    {
        var user = _context.Users.Find(userId);
        if (user == null)
        {
            throw new InvalidOperationException("User not found.");
        }

        var token = Guid.NewGuid().ToString();
        var sessionToken = new SessionToken
        {
            Token = token,
            ExpiryDate = expiryDate,
            UserId = userId
        };

        _context.SessionTokens.Add(sessionToken);
        _context.SaveChanges();

        return sessionToken;
    }

    /// <summary>
    /// Retrieves a session token by its value.
    /// </summary>
    public SessionToken GetSessionToken(string token)
    {
        return _context.SessionTokens.FirstOrDefault(t => t.Token == token && t.ExpiryDate > DateTime.UtcNow);
    }

    /// <summary>
    /// Invalidates a session token.
    /// </summary>
    public void InvalidateSessionToken(string token)
    {
        var sessionToken = _context.SessionTokens.FirstOrDefault(t => t.Token == token);
        if (sessionToken == null)
        {
            _logger.LogWarning($"Session token '{token}' not found.");
            return;
        }

        sessionToken.ExpiryDate = DateTime.UtcNow;
        _context.SaveChanges();
    }

    /// <summary>
    /// Retrieves all active session tokens for a user.
    /// </summary>
    public List<SessionToken> GetUserSessionTokens(string userId)
    {
        return _context.SessionTokens
            .Where(t => t.UserId == userId && t.ExpiryDate > DateTime.UtcNow)
            .ToList();
    }

    /// <summary>
    /// Invalidates all session tokens for a user.
    /// </summary>
    public void InvalidateAllUserSessionTokens(string userId)
    {
        var tokens = _context.SessionTokens.Where(t => t.UserId == userId).ToList();
        foreach (var token in tokens)
        {
            token.ExpiryDate = DateTime.UtcNow;
        }

        _context.SaveChanges();
    }
}