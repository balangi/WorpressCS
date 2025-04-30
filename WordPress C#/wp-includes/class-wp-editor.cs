using System;
using System.Collections.Generic;
using System.Text.Json;

public class EditorManager
{
    private readonly AppDbContext _context;

    public EditorManager(AppDbContext context = null)
    {
        _context = context;
    }

    // تنظیمات پیش‌فرض ویرایشگر
    public Dictionary<string, object> DefaultSettings(string editorId)
    {
        return new Dictionary<string, object>
        {
            { "editor_id", editorId },
            { "tinymce", true },
            { "quicktags", true },
            { "media_buttons", true },
            { "textarea_rows", 20 },
            { "teeny", false },
            { "custom_css", "" }
        };
    }

    // تولید HTML برای ویرایشگر
    public string GenerateEditorHtml(string editorId, string content, Dictionary<string, object> settings = null)
    {
        settings ??= DefaultSettings(editorId);

        var tinymceEnabled = settings.ContainsKey("tinymce") && (bool)settings["tinymce"];
        var quicktagsEnabled = settings.ContainsKey("quicktags") && (bool)settings["quicktags"];
        var customCss = settings.ContainsKey("custom_css") ? settings["custom_css"].ToString() : "";

        var htmlBuilder = new System.Text.StringBuilder();
        htmlBuilder.AppendLine($"<div id=\"wp-{editorId}-wrap\" class=\"wp-editor-wrap\">");

        if (!string.IsNullOrEmpty(customCss))
        {
            htmlBuilder.AppendLine($"<style>{customCss}</style>");
        }

        if (tinymceEnabled)
        {
            htmlBuilder.AppendLine("<script src=\"/path/to/tinymce.min.js\"></script>");
        }

        if (quicktagsEnabled)
        {
            htmlBuilder.AppendLine("<script src=\"/path/to/quicktags.min.js\"></script>");
        }

        htmlBuilder.AppendLine($"<textarea id=\"{editorId}\" name=\"{editorId}\" rows=\"{settings["textarea_rows"]}\">{content}</textarea>");
        htmlBuilder.AppendLine("</div>");

        return htmlBuilder.ToString();
    }

    // ذخیره تنظیمات ویرایشگر (اگر نیاز به ذخیره‌سازی باشد)
    public void SaveEditorSettings(EditorSetting setting)
    {
        if (_context != null)
        {
            _context.EditorSettings.Add(setting);
            _context.SaveChanges();
        }
    }

    // دریافت تنظیمات ویرایشگر (اگر نیاز به ذخیره‌سازی باشد)
    public EditorSetting GetEditorSettings(int id)
    {
        return _context?.EditorSettings.FirstOrDefault(s => s.Id == id);
    }
}