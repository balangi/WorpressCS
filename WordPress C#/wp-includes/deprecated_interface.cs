public interface IDeprecatedService
{
    void LogDeprecatedFunction(string functionName, string replacement = null, string version = null);
    List<DeprecatedFunction> GetAll();
    DeprecatedFunction GetByName(string name);
    void RemoveFunction(string name);
}