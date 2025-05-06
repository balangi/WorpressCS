using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class MatchesMapRegex
{
    /// <summary>
    /// ذخیره مقادیر $matches[]
    /// </summary>
    private readonly Dictionary<int, string> _matches;

    /// <summary>
    /// رشته‌ای که عملیات جایگزینی روی آن انجام می‌شود
    /// </summary>
    private readonly string _subject;

    /// <summary>
    /// نتیجه جایگزینی
    /// </summary>
    public string Output { get; private set; }

    /// <summary>
    /// الگوی منظم برای شناسایی $matches[]
    /// </summary>
    private static readonly Regex Pattern = new Regex(@"(\$matches\[[1-9]+[0-9]*\])");

    /// <summary>
    /// سازنده کلاس
    /// </summary>
    public MatchesMapRegex(string subject, Dictionary<int, string> matches)
    {
        _subject = subject;
        _matches = matches;
        Output = Map();
    }

    /// <summary>
    /// متد استاتیک برای ساده‌سازی استفاده
    /// </summary>
    public static string Apply(string subject, Dictionary<int, string> matches)
    {
        var instance = new MatchesMapRegex(subject, matches);
        return instance.Output;
    }

    /// <summary>
    /// اجرای عملیات جایگزینی
    /// </summary>
    private string Map()
    {
        return Pattern.Replace(_subject, Callback);
    }

    /// <summary>
    /// متد Callback برای جایگزینی مقادیر
    /// </summary>
    private string Callback(Match match)
    {
        // استخراج شماره اندیس از $matches[]
        var indexString = match.Value.Substring(9, match.Value.Length - 10);
        if (int.TryParse(indexString, out int index) && _matches.ContainsKey(index))
        {
            return Uri.EscapeDataString(_matches[index]);
        }

        return string.Empty;
    }
}