using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.Extensions.Logging;

public abstract class ImageEditorManager
{
    /// <summary>
    /// مسیر فایل تصویر
    /// </summary>
    protected string File { get; set; }

    /// <summary>
    /// اندازه تصویر
    /// </summary>
    protected Size Size { get; set; }

    /// <summary>
    /// نوع MIME تصویر
    /// </summary>
    protected string MimeType { get; set; }

    /// <summary>
    /// نوع MIME خروجی
    /// </summary>
    protected string OutputMimeType { get; set; }

    /// <summary>
    /// کیفیت پیش‌فرض تصویر
    /// </summary>
    protected int DefaultQuality { get; set; } = 82;

    /// <summary>
    /// سازنده کلاس
    /// </summary>
    public ImageEditorManager(string file)
    {
        File = file;
    }

    /// <summary>
    /// تست پشتیبانی از محیط فعلی
    /// </summary>
    public abstract bool Test();

    /// <summary>
    /// تست پشتیبانی از نوع MIME
    /// </summary>
    public abstract bool SupportsMimeType(string mimeType);

    /// <summary>
    /// بارگذاری تصویر
    /// </summary>
    public abstract void Load();

    /// <summary>
    /// تنظیم کیفیت تصویر
    /// </summary>
    public virtual void SetQuality(int quality)
    {
        if (quality < 0 || quality > 100)
        {
            throw new ArgumentException("Quality must be between 0 and 100.");
        }
        DefaultQuality = quality;
    }

    /// <summary>
    /// ذخیره‌سازی تصویر
    /// </summary>
    public virtual void Save(string outputPath)
    {
        using (var image = Image.FromFile(File))
        {
            var encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, DefaultQuality);

            var jpegEncoder = ImageCodecInfo.GetImageEncoders()
                .FirstOrDefault(codec => codec.FormatID == ImageFormat.Jpeg.Guid);

            if (jpegEncoder == null)
            {
                throw new InvalidOperationException("JPEG encoder not found.");
            }

            image.Save(outputPath, jpegEncoder, encoderParameters);
        }
    }

    /// <summary>
    /// چرخش تصویر
    /// </summary>
    public virtual void Rotate(float angle)
    {
        using (var image = Image.FromFile(File))
        {
            image.RotateFlip((RotateFlipType)angle);
            image.Save(File);
        }
    }

    /// <summary>
    /// فلیپ تصویر
    /// </summary>
    public virtual void Flip(bool horizontal, bool vertical)
    {
        using (var image = Image.FromFile(File))
        {
            var flipType = horizontal && vertical ? RotateFlipType.RotateNoneFlipXY :
                           horizontal ? RotateFlipType.RotateNoneFlipX :
                           vertical ? RotateFlipType.RotateNoneFlipY :
                           RotateFlipType.RotateNoneFlipNone;

            image.RotateFlip(flipType);
            image.Save(File);
        }
    }
}