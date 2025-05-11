using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class WP_Text_Diff_Renderer_Table : Text_Diff_Renderer
{
    /// <summary>
    /// تعداد خطوط قبل از متن تفاوت.
    /// </summary>
    public int LeadingContextLines { get; set; } = 10000;

    /// <summary>
    /// تعداد خطوط بعد از متن تفاوت.
    /// </summary>
    public int TrailingContextLines { get; set; } = 10000;

    /// <summary>
    /// آستانه برای نمایش تفاوت‌ها.
    /// </summary>
    protected double DiffThreshold { get; set; } = 0.6;

    /// <summary>
    /// نام کلاس رندرر Inline.
    /// </summary>
    protected string InlineDiffRenderer { get; set; } = "WP_Text_Diff_Renderer_Inline";

    /// <summary>
    /// آیا نمایش Split View فعال است؟
    /// </summary>
    protected bool ShowSplitView { get; set; } = true;

    /// <summary>
    /// کش برای محاسبات تفاوت‌ها.
    /// </summary>
    protected Dictionary<string, int> CountCache { get; set; } = new();

    /// <summary>
    /// کش برای ذخیره تفاوت‌ها.
    /// </summary>
    protected Dictionary<string, double> DifferenceCache { get; set; } = new();

    /// <summary>
    /// رندر کردن تفاوت‌ها.
    /// </summary>
    /// <param name="orig">لیست خطوط اصلی.</param>
    /// <param name="final">لیست خطوط نهایی.</param>
    /// <returns>HTML حاوی تفاوت‌ها.</returns>
    public string RenderDiff(List<string> orig, List<string> final)
    {
        var result = new List<string>();

        // محاسبه تفاوت‌ها
        var diff = ComputeDiff(orig, final);

        foreach (var line in diff)
        {
            if (line.StartsWith("+"))
            {
                result.Add(AddedLine(line.Substring(1)));
            }
            else if (line.StartsWith("-"))
            {
                result.Add(DeletedLine(line.Substring(1)));
            }
            else
            {
                result.Add(ContextLine(line));
            }
        }

        return string.Join(Environment.NewLine, result);
    }

    /// <summary>
    /// محاسبه تفاوت‌ها.
    /// </summary>
    /// <param name="orig">لیست خطوط اصلی.</param>
    /// <param name="final">لیست خطوط نهایی.</param>
    /// <returns>لیست خطوط تفاوت.</returns>
    private List<string> ComputeDiff(List<string> orig, List<string> final)
    {
        var diff = new List<string>();

        // مقایسه خطوط
        foreach (var line in orig)
        {
            if (!final.Contains(line))
            {
                diff.Add($"-{line}");
            }
        }

        foreach (var line in final)
        {
            if (!orig.Contains(line))
            {
                diff.Add($"+{line}");
            }
        }

        return diff;
    }

    /// <summary>
    /// خطوط اضافه شده.
    /// </summary>
    /// <param name="line">خط متن.</param>
    /// <returns>HTML خط اضافه شده.</returns>
    public string AddedLine(string line)
    {
        return $"<td class='diff-addedline'><span aria-hidden='true' class='dashicons dashicons-plus'></span><span class='screen-reader-text'>Added:</span>{line}</td>";
    }

    /// <summary>
    /// خطوط حذف شده.
    /// </summary>
    /// <param name="line">خط متن.</param>
    /// <returns>HTML خط حذف شده.</returns>
    public string DeletedLine(string line)
    {
        return $"<td class='diff-deletedline'><span aria-hidden='true' class='dashicons dashicons-minus'></span><span class='screen-reader-text'>Deleted:</span>{line}</td>";
    }

    /// <summary>
    /// خطوط بدون تغییر.
    /// </summary>
    /// <param name="line">خط متن.</param>
    /// <returns>HTML خط بدون تغییر.</returns>
    public string ContextLine(string line)
    {
        return $"<td class='diff-context'><span class='screen-reader-text'>Unchanged:</span>{line}</td>";
    }

    /// <summary>
    /// محاسبه فاصله بین دو رشته.
    /// </summary>
    /// <param name="string1">رشته اول.</param>
    /// <param name="string2">رشته دوم.</param>
    /// <returns>عددی که نشان‌دهنده فاصله است.</returns>
    public int ComputeStringDistance(string string1, string string2)
    {
        if (CountCache.ContainsKey(string1) && CountCache.ContainsKey(string2))
        {
            return Math.Abs(CountCache[string1] - CountCache[string2]);
        }

        CountCache[string1] = string1.Length;
        CountCache[string2] = string2.Length;

        return Math.Abs(string1.Length - string2.Length);
    }
}