using System.Linq;

public class TranslationService
{
    private readonly LocalizationDbContext _context;

    public TranslationService(LocalizationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// ترجمه متن بر اساس دامنه متنی و Locale
    /// </summary>
    public string Translate(string textDomain, string originalText)
    {
        var locale = LocalizationManager.GetCurrentLocale();
        var translation = _context.Translations
            .FirstOrDefault(t => t.TextDomain == textDomain &&
                                 t.Locale == locale &&
                                 t.OriginalText == originalText);

        return translation?.TranslatedText ?? originalText; // اگر ترجمه وجود نداشت، متن اصلی برگردانده شود
    }

    /// <summary>
    /// بارگذاری ترجمه‌ها از فایل‌های منابع (.resx)
    /// </summary>
    public void LoadTranslationsFromResource(string resourcePath, string textDomain, string locale)
    {
        var resourceManager = new System.Resources.ResourceManager(resourcePath, System.Reflection.Assembly.GetExecutingAssembly());
        var resourceSet = resourceManager.GetResourceSet(new CultureInfo(locale), true, true);

        foreach (System.Collections.DictionaryEntry entry in resourceSet)
        {
            var originalText = entry.Key.ToString();
            var translatedText = entry.Value.ToString();

            var existingTranslation = _context.Translations.FirstOrDefault(t => t.TextDomain == textDomain &&
                                                                                t.Locale == locale &&
                                                                                t.OriginalText == originalText);

            if (existingTranslation != null)
            {
                existingTranslation.TranslatedText = translatedText;
            }
            else
            {
                _context.Translations.Add(new Translation
                {
                    TextDomain = textDomain,
                    Locale = locale,
                    OriginalText = originalText,
                    TranslatedText = translatedText
                });
            }
        }

        _context.SaveChanges();
    }
}