using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.Extensions.Logging;

public class ImageEditorGD : ImageEditorManager
{
    /// <summary>
    /// تصویر GD
    /// </summary>
    protected Image Image { get; set; }

    /// <summary>
    /// سازنده کلاس
    /// </summary>
    public ImageEditorGD(string file) : base(file) { }

    /// <summary>
    /// تست پشتیبانی از محیط فعلی
    /// </summary>
    public override bool Test()
    {
        // تست پشتیبانی از کتابخانه GD
        return true; // در C#، System.Drawing همیشه در دسترس است
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
            Image = Image.FromFile(File);
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
    }

    /// <summary>
    /// ذخیره‌سازی تصویر
    /// </summary>
    public override void Save(string outputPath)
    {
        try
        {
            var encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, DefaultQuality);

            var encoder = GetImageEncoder(MimeType);
            if (encoder == null)
            {
                throw new InvalidOperationException("Image encoder not found.");
            }

            Image.Save(outputPath, encoder, encoderParameters);
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
            Image.RotateFlip(RotateFlipType.Rotate90FlipNone); // مثال برای چرخش 90 درجه
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
            var flipType = horizontal && vertical ? RotateFlipType.RotateNoneFlipXY :
                           horizontal ? RotateFlipType.RotateNoneFlipX :
                           vertical ? RotateFlipType.RotateNoneFlipY :
                           RotateFlipType.RotateNoneFlipNone;

            Image.RotateFlip(flipType);
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
            var resizedImage = new Bitmap(width, height);
            using (var graphics = Graphics.FromImage(resizedImage))
            {
                graphics.DrawImage(Image, 0, 0, width, height);
            }

            Image.Dispose();
            Image = resizedImage;
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

    /// <summary>
    /// دریافت کدک مناسب برای ذخیره‌سازی تصویر
    /// </summary>
    private ImageCodecInfo GetImageEncoder(string mimeType)
    {
        return ImageCodecInfo.GetImageEncoders()
            .FirstOrDefault(codec => codec.MimeType.Equals(mimeType, StringComparison.OrdinalIgnoreCase));
    }
}