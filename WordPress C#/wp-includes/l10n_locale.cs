using System.Globalization;

public class LocalizationManager
{
    public static string GetCurrentLocale()
    {
        return CultureInfo.CurrentCulture.Name; // مثل "en-US" یا "fa-IR"
    }

    public static void SetCurrentLocale(string locale)
    {
        CultureInfo.CurrentCulture = new CultureInfo(locale);
        CultureInfo.CurrentUICulture = new CultureInfo(locale);
    }
}