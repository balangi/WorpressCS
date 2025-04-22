// ICacheProvider.cs - رابط پایه برای ارائه‌دهندگان کش
public interface ICacheProvider
{
    bool Add(string key, object data, string group = "", int expire = 0);
    bool[] AddMultiple(Dictionary<string, object> data, string group = "", int expire = 0);
    bool Replace(string key, object data, string group = "", int expire = 0);
    bool Set(string key, object data, string group = "", int expire = 0);
    bool[] SetMultiple(Dictionary<string, object> data, string group = "", int expire = 0);
    object Get(string key, string group = "", bool force = false, out bool found);
    Dictionary<string, object> GetMultiple(IEnumerable<string> keys, string group = "", bool force = false);
    bool Delete(string key, string group = "");
    bool[] DeleteMultiple(IEnumerable<string> keys, string group = "");
    long Increment(string key, long offset = 1, string group = "");
    long Decrement(string key, long offset = 1, string group = "");
    bool Flush();
    bool FlushGroup(string group);
    void AddGlobalGroups(IEnumerable<string> groups);
    void SwitchToBlog(int blogId);
    bool Supports(string feature);
}