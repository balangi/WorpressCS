using System;
using System.IO;
using ImageMagick;
using Microsoft.Extensions.Logging;

public class ImageEditorImagick : ImageEditorManager
{
    /// <summary>
    /// تصویر Imagick
    /// </summary>
    protected MagickImage Image { get; set; }

    /// <summary>
    /// سازنده کلاس
    /// </summary>
    public ImageEditorImagick(string file) : base(file) { }

    /// <summary>
    /// تست پشتیبانی از محیط فعلی
    /// </summary>
    public override bool Test()
    {
        // تست پشتیبانی از کتابخانه Imagick
        return true; // Magick.NET همیشه در دسترس است
    }

    /// <summary>
    /// تست پشتیبانی از نوع MIME
    /// </summary>
    public override bool SupportsMimeType(string mimeType)
    {
        // تست پشتیبانی از نوع MIME
        return mimeType == "image/jpeg" || mimeType == "image/png" || mimeType == "image/gif";
    }

    /// <summary>
    /// بارگذاری تصویر
    /// </summary>
    public override void Load()
    {
        try
        {
            Image = new MagickImage(File);
            Size = new Size(Image.Width, Image.Height);
            MimeType = GetMimeType(File);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to load image: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// تنظیم کیفیت تصویر
    /// </summary>
    public override void SetQuality(int quality)
    {
        base.SetQuality(quality);
        if (Image != null)
        {
            Image.Quality = quality;
        }
    }

    /// <summary>
    /// ذخیره‌سازی تصویر
    /// </summary>
    public override void Save(string outputPath)
    {
        try
        {
            Image.Write(outputPath);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to save image: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// چرخش تصویر
    /// </summary>
    public override void Rotate(float angle)
    {
        try
        {
            Image.Rotate(angle);
            Size = new Size(Image.Width, Image.Height);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to rotate image: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// فلیپ تصویر
    /// </summary>
    public override void Flip(bool horizontal, bool vertical)
    {
        try
        {
            if (horizontal && vertical)
            {
                Image.Flip();
                Image.Flop();
            }
            else if (horizontal)
            {
                Image.Flip();
            }
            else if (vertical)
            {
                Image.Flop();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to flip image: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// تغییر اندازه تصویر
    /// </summary>
    public void Resize(int width, int height)
    {
        try
        {
            Image.Resize(width, height);
            Size = new Size(width, height);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to resize image: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// دریافت نوع MIME تصویر
    /// </summary>
    private string GetMimeType(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLower();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            _ => throw new NotSupportedException("Unsupported image format.")
        };
    }
}