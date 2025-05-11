public class WP_Theme_JSON
{
    public Dictionary<string, object> Data { get; private set; }
    public string Origin { get; private set; }

    public WP_Theme_JSON(Dictionary<string, object> data, string origin)
    {
        Data = data;
        Origin = origin;
    }

    public void Merge(WP_Theme_JSON other)
    {
        foreach (var key in other.Data.Keys)
        {
            if (Data.ContainsKey(key))
            {
                Data[key] = other.Data[key];
            }
            else
            {
                Data.Add(key, other.Data[key]);
            }
        }
    }

    public Dictionary<string, object> GetRawData()
    {
        return Data;
    }
}