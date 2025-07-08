public class RegistrationService
{
    private readonly ApplicationDbContext _context;

    public RegistrationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public bool CanUsersRegister()
    {
        var registrationSetting = _context.Settings.FirstOrDefault(s => s.Key == "Registration");
        return registrationSetting?.Value == "all" || registrationSetting?.Value == "user";
    }
}