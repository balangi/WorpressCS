public class UserService
{
    private readonly List<string> _illegalUsernames = new() { "admin", "root", "web", "www" };

    public bool IsUsernameValid(string username)
    {
        return !_illegalUsernames.Contains(username.ToLower());
    }
}