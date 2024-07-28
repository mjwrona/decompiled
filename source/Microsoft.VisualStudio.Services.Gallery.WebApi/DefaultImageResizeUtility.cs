// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.WebApi.DefaultImageResizeUtility
// Assembly: Microsoft.VisualStudio.Services.Gallery.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EE9D0AAA-B110-4AD6-813B-50FA04AC401A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Gallery.WebApi.dll

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Microsoft.VisualStudio.Services.Gallery.WebApi
{
  public class DefaultImageResizeUtility : IImageResizeUtility
  {
    private const string c_layer = "ImageResizeUtility";
    private Func<Exception, bool> m_exceptionHandler;
    private bool m_shouldCreateSmallIcons;

    public string OutputContentType => "image/png";

    public bool ShouldCreateSmallImages => this.m_shouldCreateSmallIcons;

    public DefaultImageResizeUtility(
      Func<Exception, bool> exceptionHandler,
      bool shouldCreateSmallIcons)
    {
      this.m_exceptionHandler = exceptionHandler;
      this.m_shouldCreateSmallIcons = shouldCreateSmallIcons;
    }

    public Stream ResizeImage(Stream stream, int maxHeight, int maxWidth, string contentType)
    {
      try
      {
        if (contentType != null && string.Compare(contentType, "image/svg+xml", StringComparison.InvariantCultureIgnoreCase) == 0 || stream.Length == 0L)
          return (Stream) null;
        stream.Seek(0L, SeekOrigin.Begin);
        using (Bitmap bitmap1 = new Bitmap(stream))
        {
          MemoryStream memoryStream = new MemoryStream();
          if (bitmap1.Height <= maxHeight && bitmap1.Width <= maxWidth)
          {
            stream.Seek(0L, SeekOrigin.Begin);
            return (Stream) null;
          }
          int width = bitmap1.Width;
          int height = bitmap1.Height;
          this.Scale(ref width, ref height, maxWidth, maxHeight);
          using (Bitmap bitmap2 = new Bitmap(width, height))
          {
            using (Graphics graphics = Graphics.FromImage((Image) bitmap2))
            {
              graphics.Clear(Color.Transparent);
              graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
              graphics.DrawImage((Image) bitmap1, 0, 0, width, height);
            }
            bitmap2.Save((Stream) memoryStream, ImageFormat.Png);
          }
          memoryStream.Seek(0L, SeekOrigin.Begin);
          return (Stream) memoryStream;
        }
      }
      catch (Exception ex)
      {
        if (!this.m_exceptionHandler(ex))
          return (Stream) null;
        throw;
      }
    }

    private void Scale(ref int targetWidth, ref int targetHeight, int maxWidth, int maxHeight)
    {
      if (maxWidth <= 0 && maxHeight <= 0)
        return;
      float num = 1f;
      if (maxHeight <= 0 && targetWidth > 0)
        num = (float) maxWidth / (float) targetWidth;
      else if (maxWidth <= 0 && targetHeight > 0)
        num = (float) maxHeight / (float) targetHeight;
      else if (targetWidth > 0 && targetHeight > 0)
        num = Math.Min((float) maxHeight / (float) targetHeight, (float) maxWidth / (float) targetWidth);
      targetWidth = (int) Math.Ceiling((double) targetWidth * (double) num);
      targetHeight = (int) Math.Ceiling((double) targetHeight * (double) num);
    }
  }
}
