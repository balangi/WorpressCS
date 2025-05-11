using System;
using System.Collections.Generic;
using System.Linq;

public class WP_Token_Map
{
    // نسخه ذخیره‌سازی
    public const string StorageVersion = "6.6.0";

    // طول کلید
    private int KeyLength { get; set; }

    // گروه‌ها
    private string Groups { get; set; }

    // کلمات بزرگ
    private Dictionary<string, string> LargeWords { get; set; } = new();

    // کلمات کوچک
    private string SmallWords { get; set; }

    // نگاشت کلمات کوچک
    private List<string> SmallMappings { get; set; } = new();

    /// <summary>
    /// سازنده خالی
    /// </summary>
    private WP_Token_Map() { }

    /// <summary>
    /// ایجاد یک نگاشت توکن از یک آرایه از کلید/مقدار.
    /// </summary>
    /// <param name="tokens">آرایه از کلید/مقدار.</param>
    /// <returns>شیء WP_Token_Map.</returns>
    public static WP_Token_Map FromArray(Dictionary<string, string> tokens)
    {
        var map = new WP_Token_Map();
        map.Precompute(tokens);
        return map;
    }

    /// <summary>
    /// ایجاد یک نگاشت توکن از جدول پیش‌محاسبه‌شده.
    /// </summary>
    /// <param name="state">جدول پیش‌محاسبه‌شده.</param>
    /// <returns>شیء WP_Token_Map.</returns>
    public static WP_Token_Map FromPrecomputedTable(Dictionary<string, object> state)
    {
        if (!state.ContainsKey("storage_version") ||
            !state.ContainsKey("key_length") ||
            !state.ContainsKey("groups") ||
            !state.ContainsKey("large_words") ||
            !state.ContainsKey("small_words") ||
            !state.ContainsKey("small_mappings"))
        {
            throw new ArgumentException("Missing required inputs to pre-computed WP_Token_Map.");
        }

        if ((string)state["storage_version"] != StorageVersion)
        {
            throw new ArgumentException($"Loaded version '{state["storage_version"]}' incompatible with expected version '{StorageVersion}'.");
        }

        var map = new WP_Token_Map
        {
            KeyLength = Convert.ToInt32(state["key_length"]),
            Groups = (string)state["groups"],
            LargeWords = (Dictionary<string, string>)state["large_words"],
            SmallWords = (string)state["small_words"],
            SmallMappings = (List<string>)state["small_mappings"]
        };

        return map;
    }

    /// <summary>
    /// پیش‌محاسبه نگاشت توکن.
    /// </summary>
    /// <param name="tokens">آرایه از کلید/مقدار.</param>
    private void Precompute(Dictionary<string, string> tokens)
    {
        foreach (var token in tokens)
        {
            if (token.Key.Length > KeyLength)
            {
                LargeWords[token.Key] = token.Value;
            }
            else
            {
                SmallWords += token.Key + "\x00";
                SmallMappings.Add(token.Value);
            }
        }
    }

    /// <summary>
    /// بررسی وجود یک توکن در نگاشت.
    /// </summary>
    /// <param name="token">توکن.</param>
    /// <returns>آیا توکن وجود دارد؟</returns>
    public bool Contains(string token)
    {
        return LargeWords.ContainsKey(token) || SmallWords.Contains(token + "\x00");
    }

    /// <summary>
    /// خواندن یک توکن از متن.
    /// </summary>
    /// <param name="text">متن.</param>
    /// <param name="position">موقعیت شروع.</param>
    /// <param name="length">طول توکن.</param>
    /// <returns>مقدار توکن.</returns>
    public string ReadToken(string text, int position, out int length)
    {
        length = 0;

        foreach (var largeWord in LargeWords.Keys)
        {
            if (text.Substring(position).StartsWith(largeWord))
            {
                length = largeWord.Length;
                return LargeWords[largeWord];
            }
        }

        foreach (var smallWord in SmallWords.Split('\x00'))
        {
            if (text.Substring(position).StartsWith(smallWord))
            {
                length = smallWord.Length;
                var index = SmallWords.Split('\x00').ToList().IndexOf(smallWord);
                return SmallMappings[index];
            }
        }

        return null;
    }

    /// <summary>
    /// صادر کردن جدول پیش‌محاسبه‌شده به صورت کد PHP.
    /// </summary>
    /// <param name="indent">فضای خالی برای تورفتگی.</param>
    /// <returns>کد PHP.</returns>
    public string PrecomputedPhpSourceTable(string indent = "\t")
    {
        var i1 = indent;
        var i2 = i1 + indent;
        var i3 = i2 + indent;

        var output = $"{i1}array(\n";
        output += $"{i2}\"storage_version\" => \"{StorageVersion}\",\n";
        output += $"{i2}\"key_length\" => {KeyLength},\n";
        output += $"{i2}\"groups\" => \"{Groups}\",\n";
        output += $"{i2}\"large_words\" => array(),\n";
        output += $"{i2}\"small_words\" => \"{SmallWords}\",\n";
        output += $"{i2}\"small_mappings\" => array(\n";

        foreach (var mapping in SmallMappings)
        {
            output += $"{i3}\"{mapping}\",\n";
        }

        output += $"{i2}),\n";
        output += $"{i1})";

        return output;
    }
}