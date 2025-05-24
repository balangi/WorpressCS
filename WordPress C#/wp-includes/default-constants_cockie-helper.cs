using System.Security.Cryptography;
using System.Text;

public static class CookieHashHelper
{
    public static string GetCookieHash(string siteUrl)
    {
        if (string.IsNullOrEmpty(siteUrl)) return "";

        using (var md5 = MD5.Create())
        {
            var inputBytes = Encoding.UTF8.GetBytes(siteUrl);
            var hashBytes = md5.ComputeHash(inputBytes);

            // Convert byte array to hex string
            StringBuilder sb = new();
            foreach (var b in hashBytes)
            {
                sb.AppendFormat("{0:x2}", b);
            }

            return sb.ToString();
        }
    }
}