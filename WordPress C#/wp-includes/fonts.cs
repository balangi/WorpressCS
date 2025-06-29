using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public class FontService
{
    private readonly ApplicationDbContext _context;

    public FontService(ApplicationDbContext context)
    {
        _context = context;
    }

    public string GenerateFontFaceStyles(List<FontFace> fonts = null)
    {
        if (fonts == null || !fonts.Any())
        {
            fonts = GetFontsFromThemeJson();
        }

        if (!fonts.Any())
        {
            return string.Empty;
        }

        var cssBuilder = new StringBuilder();

        foreach (var font in fonts)
        {
            cssBuilder.AppendLine("@font-face {");
            cssBuilder.AppendLine($"    font-family: '{font.FontFamily}';");
            cssBuilder.AppendLine($"    src: {string.Join(", ", font.Src.Select(src => $"url('{src}')"))};");

            if (!string.IsNullOrEmpty(font.FontStyle))
                cssBuilder.AppendLine($"    font-style: {font.FontStyle};");

            if (!string.IsNullOrEmpty(font.FontWeight))
                cssBuilder.AppendLine($"    font-weight: {font.FontWeight};");

            if (!string.IsNullOrEmpty(font.FontDisplay))
                cssBuilder.AppendLine($"    font-display: {font.FontDisplay};");

            if (!string.IsNullOrEmpty(font.AscentOverride))
                cssBuilder.AppendLine($"    ascent-override: {font.AscentOverride};");

            if (!string.IsNullOrEmpty(font.DescentOverride))
                cssBuilder.AppendLine($"    descent-override: {font.DescentOverride};");

            if (!string.IsNullOrEmpty(font.FontStretch))
                cssBuilder.AppendLine($"    font-stretch: {font.FontStretch};");

            if (!string.IsNullOrEmpty(font.FontVariant))
                cssBuilder.AppendLine($"    font-variant: {font.FontVariant};");

            if (!string.IsNullOrEmpty(font.FontFeatureSettings))
                cssBuilder.AppendLine($"    font-feature-settings: {font.FontFeatureSettings};");

            if (!string.IsNullOrEmpty(font.FontVariationSettings))
                cssBuilder.AppendLine($"    font-variation-settings: {font.FontVariationSettings};");

            if (!string.IsNullOrEmpty(font.LineGapOverride))
                cssBuilder.AppendLine($"    line-gap-override: {font.LineGapOverride};");

            if (!string.IsNullOrEmpty(font.SizeAdjust))
                cssBuilder.AppendLine($"    size-adjust: {font.SizeAdjust};");

            if (!string.IsNullOrEmpty(font.UnicodeRange))
                cssBuilder.AppendLine($"    unicode-range: {font.UnicodeRange};");

            cssBuilder.AppendLine("}");
        }

        return cssBuilder.ToString();
    }

    public void RegisterFontCollection(string slug, FontCollection collection)
    {
        if (_context.FontCollections.Any(fc => fc.Slug == slug))
        {
            throw new InvalidOperationException("Font collection with this slug already exists.");
        }

        _context.FontCollections.Add(collection);
        _context.SaveChanges();
    }

    public void UnregisterFontCollection(string slug)
    {
        var collection = _context.FontCollections.FirstOrDefault(fc => fc.Slug == slug);
        if (collection != null)
        {
            _context.FontCollections.Remove(collection);
            _context.SaveChanges();
        }
    }

    public Dictionary<string, string> GetFontDirectoryInfo()
    {
        var fontDir = Path.Combine(Directory.GetCurrentDirectory(), "fonts");
        var fontUrl = "/fonts";

        return new Dictionary<string, string>
        {
            { "path", fontDir },
            { "url", fontUrl }
        };
    }

    private List<FontFace> GetFontsFromThemeJson()
    {
        // Replace with actual logic to fetch fonts from theme.json
        return new List<FontFace>();
    }
}