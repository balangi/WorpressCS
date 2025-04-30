public class FeedCacheTransient
{
    public string Location { get; }
    public string Filename { get; }
    public string Extension { get; }

    public FeedCacheTransient(string location, string filename, string extension)
    {
        Location = location;
        Filename = filename;
        Extension = extension;
    }

    // شبیه‌سازی عملیات کش‌سازی
    public void SaveData(object data)
    {
        Console.WriteLine($"Saving data for feed: {Location}, Filename: {Filename}, Extension: {Extension}");
        // اینجا می‌توانید عملیات ذخیره‌سازی واقعی را انجام دهید
    }
}