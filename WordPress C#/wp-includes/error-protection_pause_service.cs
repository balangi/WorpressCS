public class PausedExtensionStorage
{
    private readonly Dictionary<string, object> _pausedExtensions = new();

    public void Pause(string key, object extension)
    {
        _pausedExtensions[key] = extension;
    }

    public object Resume(string key)
    {
        if (_pausedExtensions.TryGetValue(key, out var ext))
        {
            _pausedExtensions.Remove(key);
            return ext;
        }

        return null;
    }

    public bool IsPaused(string key)
    {
        return _pausedExtensions.ContainsKey(key);
    }

    public List<string> GetAllPaused()
    {
        return _pausedExtensions.Keys.ToList();
    }
}