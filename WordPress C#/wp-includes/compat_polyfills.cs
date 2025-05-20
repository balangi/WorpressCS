public static class Polyfills
{
    // str_contains()
    public static bool StrContains(string haystack, string needle)
    {
        if (string.IsNullOrEmpty(needle)) return true;
        return haystack?.Contains(needle) ?? false;
    }

    // str_starts_with()
    public static bool StrStartsWith(string haystack, string needle)
    {
        if (string.IsNullOrEmpty(needle)) return true;
        return haystack?.StartsWith(needle) ?? false;
    }

    // str_ends_with()
    public static bool StrEndsWith(string haystack, string needle)
    {
        if (string.IsNullOrEmpty(needle)) return true;
        return haystack?.EndsWith(needle) ?? false;
    }

    // array_key_first()
    public static TKey ArrayKeyFirst<TKey, TValue>(Dictionary<TKey, TValue> dictionary)
    {
        if (dictionary == null || dictionary.Count == 0)
            return default;

        foreach (var key in dictionary.Keys)
        {
            return key;
        }

        return default;
    }

    // array_key_last()
    public static TKey ArrayKeyLast<TKey, TValue>(Dictionary<TKey, TValue> dictionary)
    {
        if (dictionary == null || dictionary.Count == 0)
            return default;

        TKey last = default;
        foreach (var key in dictionary.Keys)
        {
            last = key;
        }

        return last;
    }

    // is_countable()
    public static bool IsCountable(object obj)
    {
        if (obj == null) return false;

        return obj is Array ||
               obj is ICollection ||
               obj is IEnumerable ||
               obj is Countable;
    }

    // mb_strlen() - UTF-8 support only
    public static int MbStrlen(string input, string encoding = "utf-8")
    {
        if (input == null) return 0;
        if (encoding.ToLower() != "utf-8") encoding = "utf-8";

        return Encoding.UTF8.GetByteCount(input);
    }

    // array_is_list() - Checks for list-like array
    public static bool ArrayIsList<T>(IList<T> list)
    {
        if (list == null || list.Count == 0) return true;

        for (int i = 0; i < list.Count; i++)
        {
            if (!(list[i] is T)) return false;
        }

        return true;
    }

    // array_find()
    public static T ArrayFind<T>(IEnumerable<T> items, Func<T, bool> predicate)
    {
        if (items == null || predicate == null) return default;

        foreach (var item in items)
        {
            if (predicate(item))
                return item;
        }

        return default;
    }

    // array_find_key()
    public static TKey ArrayFindKey<TKey, TValue>(IDictionary<TKey, TValue> dictionary, Func<TValue, TKey, bool> predicate)
    {
        if (dictionary == null || predicate == null) return default;

        foreach (var kvp in dictionary)
        {
            if (predicate(kvp.Value, kvp.Key))
                return kvp.Key;
        }

        return default;
    }

    // array_any()
    public static bool ArrayAny<T>(IEnumerable<T> items, Func<T, bool> predicate)
    {
        if (items == null || predicate == null) return false;

        foreach (var item in items)
        {
            if (predicate(item))
                return true;
        }

        return false;
    }

    // array_all()
    public static bool ArrayAll<T>(IEnumerable<T> items, Func<T, bool> predicate)
    {
        if (items == null || predicate == null) return true;

        foreach (var item in items)
        {
            if (!predicate(item))
                return false;
        }

        return true;
    }

    // mb_substr() - UTF-8 only
    public static string MbSubstr(string str, int start, int length = int.MaxValue)
    {
        if (string.IsNullOrEmpty(str)) return "";

        try
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            var subBytes = bytes.Skip(start).Take(length).ToArray();
            return Encoding.UTF8.GetString(subBytes);
        }
        catch
        {
            return "";
        }
    }

    // _is_utf8_charset()
    public static bool IsUtf8Charset(string charset)
    {
        if (string.IsNullOrWhiteSpace(charset)) return false;

        return charset.Equals("UTF-8", StringComparison.OrdinalIgnoreCase) ||
               charset.Equals("utf8", StringComparison.OrdinalIgnoreCase);
    }
}