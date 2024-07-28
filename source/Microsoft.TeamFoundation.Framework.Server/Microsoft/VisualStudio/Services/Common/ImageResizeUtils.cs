// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.ImageResizeUtils
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Microsoft.VisualStudio.Services.Common
{
  public static class ImageResizeUtils
  {
    private const long c_jpegCompressionLevel = 80;

    public static byte[] ResizeWhileMaintainingAspectRatio(
      Stream imageDataToResize,
      int widthAndHeight,
      AvatarImageFormat imageFormat)
    {
      Size desiredSize = widthAndHeight > 0 ? new Size(widthAndHeight, widthAndHeight) : throw new ArgumentException("The desired widthAndHeight must have a valid (positive) value.", nameof (widthAndHeight));
      if (imageFormat == AvatarImageFormat.Png)
        return ImageResizeUtils.ResizeImageStream(imageDataToResize, desiredSize, ImageFormat.Png, true);
      if (imageFormat == AvatarImageFormat.Jpeg)
        return ImageResizeUtils.ResizeImageStream(imageDataToResize, desiredSize, ImageFormat.Jpeg, true);
      throw new InvalidOperationException();
    }

    public static byte[] ResizeImageStream(
      Stream imageDataToResize,
      Size desiredSize,
      ImageFormat outputFormat,
      bool constrainToAspectRatio = false)
    {
      if (desiredSize.Width <= 0 || desiredSize.Height <= 0)
        throw new ArgumentException("The desired size must have a valid (positive) width and height.", nameof (desiredSize));
      using (Image image = Image.FromStream(imageDataToResize))
      {
        if (constrainToAspectRatio)
          desiredSize = ImageResizeUtils.ConstrainSizeByAspectRatio(image.Size, desiredSize);
        using (Bitmap bitmap = new Bitmap(desiredSize.Width, desiredSize.Height))
        {
          using (Graphics graphics = Graphics.FromImage((Image) bitmap))
          {
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            RectangleF srcRect = new RectangleF(0.0f, 0.0f, (float) image.Width, (float) image.Height);
            srcRect.Offset(-0.5f, -0.5f);
            graphics.DrawImage(image, new RectangleF(0.0f, 0.0f, (float) bitmap.Width, (float) bitmap.Height), srcRect, GraphicsUnit.Pixel);
          }
          using (MemoryStream memoryStream = new MemoryStream())
          {
            if (ImageFormat.Jpeg == outputFormat)
            {
              EncoderParameters encoderParams = new EncoderParameters(1);
              encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, 80L);
              bitmap.Save((Stream) memoryStream, ImageResizeUtils.GetEncoder(outputFormat), encoderParams);
            }
            else
              bitmap.Save((Stream) memoryStream, outputFormat);
            return memoryStream.ToArray();
          }
        }
      }
    }

    public static AvatarImageFormat MapToAvatarFormat(string format)
    {
      if (string.IsNullOrEmpty(format))
        return AvatarImageFormat.Unknown;
      if (format.Equals("png", StringComparison.OrdinalIgnoreCase))
        return AvatarImageFormat.Png;
      return format.Equals("jpg", StringComparison.OrdinalIgnoreCase) || format.Equals("jpeg", StringComparison.OrdinalIgnoreCase) ? AvatarImageFormat.Jpeg : AvatarImageFormat.Unknown;
    }

    private static Size ConstrainSizeByAspectRatio(Size originalSize, Size desiredSize)
    {
      int width;
      int height;
      if (originalSize.Height >= originalSize.Width)
      {
        width = (int) ((double) (desiredSize.Height * originalSize.Width) / (double) originalSize.Height);
        height = desiredSize.Height;
      }
      else
      {
        width = desiredSize.Width;
        height = (int) ((double) (desiredSize.Width * originalSize.Height) / (double) originalSize.Width);
      }
      return new Size(width, height);
    }

    public static byte[] ResizeSingleAvatar(
      byte[] image,
      AvatarImageFormat imageType,
      int pixelSize)
    {
      using (MemoryStream imageDataToResize = new MemoryStream(image))
      {
        if (imageType == AvatarImageFormat.Png)
          return ImageResizeUtils.ResizeWhileMaintainingAspectRatio((Stream) imageDataToResize, pixelSize, AvatarImageFormat.Png);
        if (imageType == AvatarImageFormat.Jpeg)
          return ImageResizeUtils.ResizeWhileMaintainingAspectRatio((Stream) imageDataToResize, pixelSize, AvatarImageFormat.Jpeg);
        throw new ArgumentException("The argument is out of the supported enum range", nameof (imageType));
      }
    }

    private static ImageCodecInfo GetEncoder(ImageFormat imageFormat)
    {
      foreach (ImageCodecInfo imageEncoder in ImageCodecInfo.GetImageEncoders())
      {
        if (imageEncoder.FormatID == imageFormat.Guid)
          return imageEncoder;
      }
      throw new InvalidOperationException();
    }
  }
}
