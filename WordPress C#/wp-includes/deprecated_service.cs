public class DeprecatedService : IDeprecatedService
{
    private readonly ApplicationDbContext _context;

    public DeprecatedService(ApplicationDbContext context)
    {
        _context = context;
    }

    public void LogDeprecatedFunction(string functionName, string replacement = null, string version = null)
    {
        var func = new DeprecatedFunction
        {
            Name = functionName,
            Replacement = replacement,
            VersionDeprecated = version,
            VersionRemoved = null
        };

        _context.DeprecatedFunctions.Add(func);
        _context.SaveChanges();
    }

    public List<DeprecatedFunction> GetAll() =>
        _context.DeprecatedFunctions.ToList();

    public DeprecatedFunction GetByName(string name) =>
        _context.DeprecatedFunctions.FirstOrDefault(f => f.Name == name);

    public void RemoveFunction(string name)
    {
        var func = GetByName(name);
        if (func != null)
        {
            func.VersionRemoved = "7.0.0"; // Sample removed version
            _context.DeprecatedFunctions.Update(func);
            _context.SaveChanges();
        }
    }
}