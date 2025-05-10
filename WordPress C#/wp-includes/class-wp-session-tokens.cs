using System;
using System.Linq;

public abstract class SessionTokenManager
{
    protected readonly AppDbContext _context;
    protected readonly int _userId;

    protected SessionTokenManager(AppDbContext context, int userId)
    {
        _context = context;
        _userId = userId;
    }

    // دریافت نمونه مدیر جلسه برای یک کاربر
    public static SessionTokenManager GetInstance(AppDbContext context, int userId)
    {
        return new UserMetaSessionTokenManager(context, userId);
    }

    // هش کردن توکن
    private string HashToken(string token)
    {
        using (var sha256 = System.Security.Cryptography.SHA256.Create())
        {
            var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(hashBytes);
        }
    }

    // دریافت جلسه برای یک توکن
    public SessionToken Get(string token)
    {
        var verifier = HashToken(token);
        return GetSession(verifier);
    }

    // اعتبارسنجی توکن
    public bool Verify(string token)
    {
        var verifier = HashToken(token);
        return GetSession(verifier) != null;
    }

    // ایجاد یک توکن جدید
    public string Create(DateTime expiration)
    {
        var session = new SessionToken
        {
            UserId = _userId,
            TokenHash = HashToken(Guid.NewGuid().ToString()),
            Expiration = expiration,
            IpAddress = GetIpAddress(),
            UserAgent = GetUserAgent(),
            LoginTime = DateTime.UtcNow
        };

        _context.SessionTokens.Add(session);
        _context.SaveChanges();

        return session.TokenHash;
    }

    // به‌روزرسانی جلسه
    public void Update(string token, SessionToken session)
    {
        var verifier = HashToken(token);
        var existingSession = GetSession(verifier);
        if (existingSession != null)
        {
            existingSession.Expiration = session.Expiration;
            existingSession.IpAddress = session.IpAddress;
            existingSession.UserAgent = session.UserAgent;
            existingSession.LoginTime = session.LoginTime;

            _context.SaveChanges();
        }
    }

    // حذف یک جلسه
    public void Destroy(string token)
    {
        var verifier = HashToken(token);
        var session = GetSession(verifier);
        if (session != null)
        {
            _context.SessionTokens.Remove(session);
            _context.SaveChanges();
        }
    }

    // حذف تمام جلسات به جز جلسه فعلی
    public void DestroyOthers(string tokenToKeep)
    {
        var verifier = HashToken(tokenToKeep);
        var sessionToKeep = GetSession(verifier);

        if (sessionToKeep != null)
        {
            DestroyOtherSessions(verifier);
        }
        else
        {
            DestroyAllSessions();
        }
    }

    // حذف تمام جلسات برای یک کاربر
    public void DestroyAll()
    {
        DestroyAllSessions();
    }

    // حذف تمام جلسات برای تمام کاربران
    public static void DestroyAllForAllUsers(AppDbContext context)
    {
        context.SessionTokens.RemoveRange(context.SessionTokens);
        context.SaveChanges();
    }

    // دریافت تمام جلسات یک کاربر
    public List<SessionToken> GetAll()
    {
        return GetSessions().ToList();
    }

    // متد‌های انتزاعی برای پیاده‌سازی در کلاس‌های مشتق‌شده
    protected abstract SessionToken GetSession(string verifier);
    protected abstract void UpdateSession(string verifier, SessionToken session = null);
    protected abstract void DestroyOtherSessions(string verifier);
    protected abstract void DestroyAllSessions();

    // کمک‌کننده‌ها برای دریافت IP و عامل کاربر
    private string GetIpAddress()
    {
        return "127.0.0.1"; // به صورت پیش‌فرض - می‌توانید از HttpContext در ASP.NET استفاده کنید
    }

    private string GetUserAgent()
    {
        return "Sample User Agent"; // به صورت پیش‌فرض - می‌توانید از HttpContext در ASP.NET استفاده کنید
    }
}

// کلاس مشتق‌شده برای مدیریت جلسات کاربر
public class UserMetaSessionTokenManager : SessionTokenManager
{
    public UserMetaSessionTokenManager(AppDbContext context, int userId) : base(context, userId) { }

    protected override SessionToken GetSession(string verifier)
    {
        return _context.SessionTokens.FirstOrDefault(s => s.TokenHash == verifier && s.UserId == _userId);
    }

    protected override void UpdateSession(string verifier, SessionToken session = null)
    {
        var existingSession = _context.SessionTokens.FirstOrDefault(s => s.TokenHash == verifier && s.UserId == _userId);
        if (existingSession != null)
        {
            if (session == null)
            {
                _context.SessionTokens.Remove(existingSession);
            }
            else
            {
                existingSession.Expiration = session.Expiration;
                existingSession.IpAddress = session.IpAddress;
                existingSession.UserAgent = session.UserAgent;
                existingSession.LoginTime = session.LoginTime;
            }
            _context.SaveChanges();
        }
    }

    protected override void DestroyOtherSessions(string verifier)
    {
        var sessions = _context.SessionTokens.Where(s => s.UserId == _userId && s.TokenHash != verifier).ToList();
        _context.SessionTokens.RemoveRange(sessions);
        _context.SaveChanges();
    }

    protected override void DestroyAllSessions()
    {
        var sessions = _context.SessionTokens.Where(s => s.UserId == _userId).ToList();
        _context.SessionTokens.RemoveRange(sessions);
        _context.SaveChanges();
    }
}