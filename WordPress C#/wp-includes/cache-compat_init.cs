// اضافه کردن این متدها به کلاس WpCache
public static Func<Dictionary<string, object>, string, int, Dictionary<string, bool>> AddMultiple { get; set; }
public static Func<Dictionary<string, object>, string, int, Dictionary<string, bool>> SetMultiple { get; set; }
public static Func<IEnumerable<string>, string, bool, Dictionary<string, object>> GetMultiple { get; set; }
public static Func<IEnumerable<string>, string, Dictionary<string, bool>> DeleteMultiple { get; set; }
public static Func<bool> FlushRuntime { get; set; }
public static Func<string, bool> FlushGroup { get; set; }

// در متد Init این خط را اضافه کنید
WpCacheExtended.InitCompatibility();