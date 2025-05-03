using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

public class HttpEncodingManager
{
    /// <summary>
    /// فشرده‌سازی داده‌ها با استفاده از الگوریتم Deflate
    /// </summary>
    public static string Compress(string raw, int level = 9)
    {
        try
        {
            var bytes = Encoding.UTF8.GetBytes(raw);
            using (var memoryStream = new MemoryStream())
            {
                using (var deflateStream = new DeflateStream(memoryStream, CompressionLevel.Optimal))
                {
                    deflateStream.Write(bytes, 0, bytes.Length);
                }
                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Compression failed: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// بازکردن داده‌های فشرده‌شده
    /// </summary>
    public static string Decompress(string compressed)
    {
        if (string.IsNullOrEmpty(compressed))
        {
            return compressed;
        }

        try
        {
            var bytes = Convert.FromBase64String(compressed);
            using (var memoryStream = new MemoryStream(bytes))
            using (var deflateStream = new DeflateStream(memoryStream, CompressionMode.Decompress))
            using (var output = new MemoryStream())
            {
                deflateStream.CopyTo(output);
                return Encoding.UTF8.GetString(output.ToArray());
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Decompression failed: {ex.Message}");
            return compressed;
        }
    }

    /// <summary>
    /// بازکردن داده‌های فشرده‌شده با سازگاری با سرورهای مختلف
    /// </summary>
    public static string CompatibleGzInflate(string gzData)
    {
        if (string.IsNullOrEmpty(gzData))
        {
            return null;
        }

        try
        {
            // حذف هدر GZIP اگر وجود داشته باشد
            if (gzData.StartsWith("\x1f\x8b\x08"))
            {
                var data = Convert.FromBase64String(gzData);
                var decompressed = Decompress(Convert.ToBase64String(data[10..^8]));
                if (!string.IsNullOrEmpty(decompressed))
                {
                    return decompressed;
                }
            }

            // سعی در بازکردن داده‌ها بدون هدر
            var result = Decompress(gzData);
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Compatible decompression failed: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// تعیین انواع فشرده‌سازی قابل قبول
    /// </summary>
    public static string AcceptEncoding(string url, Dictionary<string, object> args)
    {
        var types = new List<string>();

        if (args.ContainsKey("decompress") && !(bool)args["decompress"])
        {
            return string.Join(", ", types);
        }

        if (IsAvailable())
        {
            types.Add("deflate;q=1.0");
            types.Add("gzip;q=0.5");
        }

        return string.Join(", ", types);
    }

    /// <summary>
    /// بررسی اینکه آیا فشرده‌سازی و بازکردن داده‌ها پشتیبانی می‌شود
    /// </summary>
    public static bool IsAvailable()
    {
        return true; // در C#، فشرده‌سازی و بازکردن داده‌ها به صورت پیش‌فرض پشتیبانی می‌شود
    }

    /// <summary>
    /// بررسی اینکه آیا داده‌ها باید بازکردن شوند بر اساس هدرها
    /// </summary>
    public static bool ShouldDecode(Dictionary<string, string> headers)
    {
        if (headers.ContainsKey("Content-Encoding") && !string.IsNullOrEmpty(headers["Content-Encoding"]))
        {
            return true;
        }

        return false;
    }
}