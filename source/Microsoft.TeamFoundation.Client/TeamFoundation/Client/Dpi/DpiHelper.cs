// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.Dpi.DpiHelper
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Microsoft.TeamFoundation.Client.Dpi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class DpiHelper
  {
    private static DpiHelper _default;
    protected const double DefaultLogicalDpi = 96.0;
    private ImageScalingMode _imageScalingMode;
    private BitmapScalingMode _bitmapScalingMode;
    private bool? _usePreScaledImages;
    private MatrixTransform transformFromDevice;
    private MatrixTransform transformToDevice;
    private double _preScaledImageLayoutTransformScaleX;
    private double _preScaledImageLayoutTransformScaleY;

    public static DpiHelper Default
    {
      get
      {
        if (DpiHelper._default == null)
          DpiHelper._default = DpiHelper.GetHelper(100);
        return DpiHelper._default;
      }
    }

    public static DpiHelper GetHelper(int zoomPercent) => new DpiHelper(96.0 * (double) zoomPercent / 100.0);

    protected DpiHelper(double logicalDpi)
    {
      this.LogicalDpiX = logicalDpi;
      this.LogicalDpiY = logicalDpi;
      IntPtr dc = Microsoft.TeamFoundation.Common.Internal.NativeMethods.GetDC(IntPtr.Zero);
      if (dc != IntPtr.Zero)
      {
        this.DeviceDpiX = (double) Microsoft.TeamFoundation.Common.Internal.NativeMethods.GetDeviceCaps(dc, 88);
        this.DeviceDpiY = (double) Microsoft.TeamFoundation.Common.Internal.NativeMethods.GetDeviceCaps(dc, 90);
        Microsoft.TeamFoundation.Common.Internal.NativeMethods.ReleaseDC(IntPtr.Zero, dc);
      }
      else
      {
        this.DeviceDpiX = this.LogicalDpiX;
        this.DeviceDpiY = this.LogicalDpiY;
      }
      System.Windows.Media.Matrix identity1 = System.Windows.Media.Matrix.Identity;
      System.Windows.Media.Matrix identity2 = System.Windows.Media.Matrix.Identity;
      identity1.Scale(this.DeviceDpiX / this.LogicalDpiX, this.DeviceDpiY / this.LogicalDpiY);
      identity2.Scale(this.LogicalDpiX / this.DeviceDpiX, this.LogicalDpiY / this.DeviceDpiY);
      this.transformFromDevice = new MatrixTransform(identity2);
      this.transformFromDevice.Freeze();
      this.transformToDevice = new MatrixTransform(identity1);
      this.transformToDevice.Freeze();
    }

    private ImageScalingMode GetDefaultImageScalingMode(int dpiScalePercent)
    {
      if (dpiScalePercent % 100 == 0)
        return ImageScalingMode.NearestNeighbor;
      return dpiScalePercent < 100 ? ImageScalingMode.HighQualityBilinear : ImageScalingMode.MixedNearestNeighborHighQualityBicubic;
    }

    private BitmapScalingMode GetDefaultBitmapScalingMode(int dpiScalePercent)
    {
      if (dpiScalePercent % 100 == 0)
        return BitmapScalingMode.NearestNeighbor;
      return dpiScalePercent < 100 ? BitmapScalingMode.LowQuality : BitmapScalingMode.HighQuality;
    }

    protected virtual ImageScalingMode GetImageScalingModeOverride(
      int dpiScalePercent,
      ImageScalingMode defaultImageScalingMode)
    {
      return defaultImageScalingMode;
    }

    protected virtual BitmapScalingMode GetBitmapScalingModeOverride(
      int dpiScalePercent,
      BitmapScalingMode defaultBitmapScalingMode)
    {
      return defaultBitmapScalingMode;
    }

    protected virtual bool GetUsePreScaledImagesOverride(
      int dpiScalePercent,
      bool defaultUsePreScaledImages)
    {
      return defaultUsePreScaledImages;
    }

    public ImageScalingMode ImageScalingMode
    {
      get
      {
        if (this._imageScalingMode == ImageScalingMode.Default)
        {
          int dpiScalePercentX = this.DpiScalePercentX;
          ImageScalingMode imageScalingMode = this.GetDefaultImageScalingMode(dpiScalePercentX);
          this._imageScalingMode = this.GetImageScalingModeOverride(dpiScalePercentX, imageScalingMode);
          if (!Enum.IsDefined(typeof (ImageScalingMode), (object) this._imageScalingMode) || this._imageScalingMode == ImageScalingMode.Default)
            this._imageScalingMode = imageScalingMode;
        }
        return this._imageScalingMode;
      }
    }

    private InterpolationMode GetInterpolationMode(ImageScalingMode scalingMode)
    {
      switch (scalingMode)
      {
        case ImageScalingMode.BorderOnly:
        case ImageScalingMode.NearestNeighbor:
          return InterpolationMode.NearestNeighbor;
        case ImageScalingMode.Bilinear:
          return InterpolationMode.Bilinear;
        case ImageScalingMode.Bicubic:
          return InterpolationMode.Bicubic;
        case ImageScalingMode.HighQualityBilinear:
          return InterpolationMode.HighQualityBilinear;
        case ImageScalingMode.HighQualityBicubic:
          return InterpolationMode.HighQualityBicubic;
        default:
          return this.GetInterpolationMode(this.ImageScalingMode);
      }
    }

    private ImageScalingMode GetActualScalingMode(ImageScalingMode scalingMode) => scalingMode != ImageScalingMode.Default ? scalingMode : this.ImageScalingMode;

    public BitmapScalingMode BitmapScalingMode
    {
      get
      {
        if (this._bitmapScalingMode == BitmapScalingMode.Unspecified)
        {
          int dpiScalePercentX = this.DpiScalePercentX;
          BitmapScalingMode bitmapScalingMode = this.GetDefaultBitmapScalingMode(dpiScalePercentX);
          this._bitmapScalingMode = this.GetBitmapScalingModeOverride(dpiScalePercentX, bitmapScalingMode);
          if (!Enum.IsDefined(typeof (BitmapScalingMode), (object) this._bitmapScalingMode) || this._bitmapScalingMode == BitmapScalingMode.Unspecified)
            this._bitmapScalingMode = bitmapScalingMode;
        }
        return this._bitmapScalingMode;
      }
    }

    public bool UsePreScaledImages
    {
      get
      {
        if (!this._usePreScaledImages.HasValue)
          this._usePreScaledImages = new bool?(this.GetUsePreScaledImagesOverride(this.DpiScalePercentX, true));
        return this._usePreScaledImages.Value;
      }
    }

    public MatrixTransform TransformFromDevice => this.transformFromDevice;

    public MatrixTransform TransformToDevice => this.transformToDevice;

    public double DeviceDpiX { get; private set; }

    public double DeviceDpiY { get; private set; }

    public double LogicalDpiX { get; private set; }

    public double LogicalDpiY { get; private set; }

    public bool IsScalingRequired => this.DeviceDpiX != this.LogicalDpiX || this.DeviceDpiY != this.LogicalDpiY;

    public double DeviceToLogicalUnitsScalingFactorX => this.TransformFromDevice.Matrix.M11;

    public double DeviceToLogicalUnitsScalingFactorY => this.TransformFromDevice.Matrix.M22;

    public double LogicalToDeviceUnitsScalingFactorX => this.TransformToDevice.Matrix.M11;

    public double LogicalToDeviceUnitsScalingFactorY => this.TransformToDevice.Matrix.M22;

    public double LogicalToDeviceUnitsX(double value) => value * this.LogicalToDeviceUnitsScalingFactorX;

    public double LogicalToDeviceUnitsY(double value) => value * this.LogicalToDeviceUnitsScalingFactorY;

    public double DeviceToLogicalUnitsX(double value) => value * this.DeviceToLogicalUnitsScalingFactorX;

    public double DeviceToLogicalUnitsY(double value) => value * this.DeviceToLogicalUnitsScalingFactorY;

    public float LogicalToDeviceUnitsX(float value) => (float) this.LogicalToDeviceUnitsX((double) value);

    public float LogicalToDeviceUnitsY(float value) => (float) this.LogicalToDeviceUnitsY((double) value);

    public int LogicalToDeviceUnitsX(int value) => (int) Math.Round(this.LogicalToDeviceUnitsX((double) value));

    public int LogicalToDeviceUnitsY(int value) => (int) Math.Round(this.LogicalToDeviceUnitsY((double) value));

    public float DeviceToLogicalUnitsX(float value) => value * (float) this.DeviceToLogicalUnitsScalingFactorX;

    public float DeviceToLogicalUnitsY(float value) => value * (float) this.DeviceToLogicalUnitsScalingFactorY;

    public int DeviceToLogicalUnitsX(int value) => (int) Math.Round((double) value * this.DeviceToLogicalUnitsScalingFactorX);

    public int DeviceToLogicalUnitsY(int value) => (int) Math.Round((double) value * this.DeviceToLogicalUnitsScalingFactorY);

    public double RoundToDeviceUnitsX(double value) => this.DeviceToLogicalUnitsX(Math.Round(this.LogicalToDeviceUnitsX(value)));

    public double RoundToDeviceUnitsY(double value) => this.DeviceToLogicalUnitsY(Math.Round(this.LogicalToDeviceUnitsY(value)));

    public System.Windows.Point LogicalToDeviceUnits(System.Windows.Point logicalPoint) => this.TransformToDevice.Transform(logicalPoint);

    public Rect LogicalToDeviceUnits(Rect logicalRect)
    {
      Rect deviceUnits = logicalRect;
      deviceUnits.Transform(this.TransformToDevice.Matrix);
      return deviceUnits;
    }

    public System.Windows.Size LogicalToDeviceUnits(System.Windows.Size logicalSize) => new System.Windows.Size(logicalSize.Width * this.LogicalToDeviceUnitsScalingFactorX, logicalSize.Height * this.LogicalToDeviceUnitsScalingFactorY);

    public Thickness LogicalToDeviceUnits(Thickness logicalThickness) => new Thickness(logicalThickness.Left * this.LogicalToDeviceUnitsScalingFactorX, logicalThickness.Top * this.LogicalToDeviceUnitsScalingFactorY, logicalThickness.Right * this.LogicalToDeviceUnitsScalingFactorX, logicalThickness.Bottom * this.LogicalToDeviceUnitsScalingFactorY);

    public Padding LogicalToDeviceUnits(Padding logicalPadding) => new Padding((int) ((double) logicalPadding.Left * this.LogicalToDeviceUnitsScalingFactorX), (int) ((double) logicalPadding.Top * this.LogicalToDeviceUnitsScalingFactorY), (int) ((double) logicalPadding.Right * this.LogicalToDeviceUnitsScalingFactorX), (int) ((double) logicalPadding.Bottom * this.LogicalToDeviceUnitsScalingFactorY));

    public System.Windows.Point DeviceToLogicalUnits(System.Windows.Point devicePoint) => this.TransformFromDevice.Transform(devicePoint);

    public Rect DeviceToLogicalUnits(Rect deviceRect)
    {
      Rect logicalUnits = deviceRect;
      logicalUnits.Transform(this.TransformFromDevice.Matrix);
      return logicalUnits;
    }

    public System.Windows.Size DeviceToLogicalUnits(System.Windows.Size deviceSize) => new System.Windows.Size(deviceSize.Width * this.DeviceToLogicalUnitsScalingFactorX, deviceSize.Height * this.DeviceToLogicalUnitsScalingFactorY);

    public Thickness DeviceToLogicalUnits(Thickness deviceThickness) => new Thickness(deviceThickness.Left * this.DeviceToLogicalUnitsScalingFactorX, deviceThickness.Top * this.DeviceToLogicalUnitsScalingFactorY, deviceThickness.Right * this.DeviceToLogicalUnitsScalingFactorX, deviceThickness.Bottom * this.DeviceToLogicalUnitsScalingFactorY);

    public void SetDeviceLeft(ref Window window, double deviceLeft) => window.Left = deviceLeft * this.DeviceToLogicalUnitsScalingFactorX;

    public double GetDeviceLeft(Window window) => window.Left * this.LogicalToDeviceUnitsScalingFactorX;

    public void SetDeviceTop(ref Window window, double deviceTop) => window.Top = deviceTop * this.DeviceToLogicalUnitsScalingFactorY;

    public double GetDeviceTop(Window window) => window.Top * this.LogicalToDeviceUnitsScalingFactorY;

    public void SetDeviceWidth(ref Window window, double deviceWidth) => window.Width = deviceWidth * this.DeviceToLogicalUnitsScalingFactorX;

    public double GetDeviceWidth(Window window) => window.Width * this.LogicalToDeviceUnitsScalingFactorX;

    public void SetDeviceHeight(ref Window window, double deviceHeight) => window.Height = deviceHeight * this.DeviceToLogicalUnitsScalingFactorY;

    public double GetDeviceHeight(Window window) => window.Height * this.LogicalToDeviceUnitsScalingFactorY;

    public Rect GetDeviceRect(Window window) => new Rect(new System.Windows.Point(window.GetDeviceLeft(), window.GetDeviceTop()), new System.Windows.Size(window.GetDeviceWidth(), window.GetDeviceHeight()));

    public System.Windows.Size GetDeviceActualSize(FrameworkElement element) => this.LogicalToDeviceUnits(new System.Windows.Size(element.ActualWidth, element.ActualHeight));

    public System.Drawing.Point LogicalToDeviceUnits(System.Drawing.Point logicalPoint) => new System.Drawing.Point(this.LogicalToDeviceUnitsX(logicalPoint.X), this.LogicalToDeviceUnitsY(logicalPoint.Y));

    public System.Drawing.Size LogicalToDeviceUnits(System.Drawing.Size logicalSize) => new System.Drawing.Size(this.LogicalToDeviceUnitsX(logicalSize.Width), this.LogicalToDeviceUnitsY(logicalSize.Height));

    public Rectangle LogicalToDeviceUnits(Rectangle logicalRect) => new Rectangle(this.LogicalToDeviceUnitsX(logicalRect.X), this.LogicalToDeviceUnitsY(logicalRect.Y), this.LogicalToDeviceUnitsX(logicalRect.Width), this.LogicalToDeviceUnitsY(logicalRect.Height));

    public PointF LogicalToDeviceUnits(PointF logicalPoint) => new PointF(this.LogicalToDeviceUnitsX(logicalPoint.X), this.LogicalToDeviceUnitsY(logicalPoint.Y));

    public SizeF LogicalToDeviceUnits(SizeF logicalSize) => new SizeF(this.LogicalToDeviceUnitsX(logicalSize.Width), this.LogicalToDeviceUnitsY(logicalSize.Height));

    public RectangleF LogicalToDeviceUnits(RectangleF logicalRect) => new RectangleF(this.LogicalToDeviceUnitsX(logicalRect.X), this.LogicalToDeviceUnitsY(logicalRect.Y), this.LogicalToDeviceUnitsX(logicalRect.Width), this.LogicalToDeviceUnitsY(logicalRect.Height));

    public void LogicalToDeviceUnits(ref Bitmap bitmapImage, ImageScalingMode scalingMode = ImageScalingMode.Default) => this.LogicalToDeviceUnits(ref bitmapImage, System.Drawing.Color.Transparent, scalingMode);

    public void LogicalToDeviceUnits(
      ref Bitmap bitmapImage,
      System.Drawing.Color backgroundColor,
      ImageScalingMode scalingMode = ImageScalingMode.Default)
    {
      Image image = (Image) bitmapImage;
      this.LogicalToDeviceUnits(ref image, backgroundColor, scalingMode);
      bitmapImage = (Bitmap) image;
    }

    public void LogicalToDeviceUnits(ref Image image, ImageScalingMode scalingMode = ImageScalingMode.Default) => this.LogicalToDeviceUnits(ref image, System.Drawing.Color.Transparent, scalingMode);

    public void LogicalToDeviceUnits(
      ref Image image,
      System.Drawing.Color backgroundColor,
      ImageScalingMode scalingMode = ImageScalingMode.Default)
    {
      DpiHelper.Validate.IsNotNull((object) image, nameof (image));
      if (!this.IsScalingRequired)
        return;
      Image fromLogicalImage = this.CreateDeviceFromLogicalImage(image, backgroundColor, scalingMode);
      image.Dispose();
      image = fromLogicalImage;
    }

    public int DpiScalePercentX => (int) Math.Round(this.LogicalToDeviceUnitsScalingFactorX * 100.0);

    public int DpiScalePercentY => (int) Math.Round(this.LogicalToDeviceUnitsScalingFactorY * 100.0);

    private System.Drawing.Size GetPrescaledImageSize(System.Drawing.Size size) => new System.Drawing.Size(size.Width * (this.DpiScalePercentX / 100), size.Height * (this.DpiScalePercentY / 100));

    public double PreScaledImageLayoutTransformScaleX
    {
      get
      {
        if (this._preScaledImageLayoutTransformScaleX == 0.0)
        {
          if (!this.UsePreScaledImages)
          {
            this._preScaledImageLayoutTransformScaleX = 1.0;
          }
          else
          {
            int dpiScalePercentX = this.DpiScalePercentX;
            this._preScaledImageLayoutTransformScaleX = dpiScalePercentX >= 200 ? 1.0 / (double) (dpiScalePercentX / 100) : 1.0;
          }
        }
        return this._preScaledImageLayoutTransformScaleX;
      }
    }

    public double PreScaledImageLayoutTransformScaleY
    {
      get
      {
        if (this._preScaledImageLayoutTransformScaleY == 0.0)
        {
          if (!this.UsePreScaledImages)
          {
            this._preScaledImageLayoutTransformScaleY = 1.0;
          }
          else
          {
            int dpiScalePercentY = this.DpiScalePercentY;
            this._preScaledImageLayoutTransformScaleY = dpiScalePercentY >= 200 ? 1.0 / (double) (dpiScalePercentY / 100) : 1.0;
          }
        }
        return this._preScaledImageLayoutTransformScaleY;
      }
    }

    public Image CreateDeviceFromLogicalImage(Image logicalImage, ImageScalingMode scalingMode = ImageScalingMode.Default) => this.CreateDeviceFromLogicalImage(logicalImage, System.Drawing.Color.Transparent, scalingMode);

    private void ProcessBitmapPixels(Bitmap image, DpiHelper.PixelProcessor pixelProcessor)
    {
      Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
      BitmapData bitmapdata = image.LockBits(rect, ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
      try
      {
        IntPtr scan0 = bitmapdata.Scan0;
        int length = Math.Abs(bitmapdata.Stride) * bitmapdata.Height;
        byte[] numArray = new byte[length];
        Marshal.Copy(scan0, numArray, 0, length);
        for (int index = 0; index < numArray.Length; index += 4)
          pixelProcessor(ref numArray[index + 3], ref numArray[index + 2], ref numArray[index + 1], ref numArray[index]);
        Marshal.Copy(numArray, 0, scan0, length);
      }
      finally
      {
        image.UnlockBits(bitmapdata);
      }
    }

    public ImageSource ScaleLogicalImageForDeviceSize(
      ImageSource image,
      System.Windows.Size deviceImageSize,
      BitmapScalingMode scalingMode)
    {
      DpiHelper.Validate.IsNotNull((object) image, nameof (image));
      DrawingGroup target = new DrawingGroup();
      target.Children.Add((System.Windows.Media.Drawing) new ImageDrawing(image, new Rect(deviceImageSize)));
      DrawingVisual drawingVisual = new DrawingVisual();
      using (DrawingContext drawingContext = drawingVisual.RenderOpen())
      {
        RenderOptions.SetBitmapScalingMode((DependencyObject) target, scalingMode);
        drawingContext.DrawDrawing((System.Windows.Media.Drawing) target);
      }
      RenderTargetBitmap source = new RenderTargetBitmap((int) deviceImageSize.Width, (int) deviceImageSize.Height, this.LogicalDpiX, this.LogicalDpiY, PixelFormats.Default);
      source.Render((Visual) drawingVisual);
      BitmapFrame bitmapFrame = BitmapFrame.Create((BitmapSource) source);
      bitmapFrame.Freeze();
      return (ImageSource) bitmapFrame;
    }

    public Image CreateDeviceFromLogicalImage(
      Image logicalImage,
      System.Drawing.Color backgroundColor,
      ImageScalingMode scalingMode = ImageScalingMode.Default)
    {
      DpiHelper.Validate.IsNotNull((object) logicalImage, nameof (logicalImage));
      ImageScalingMode scalingMode1 = this.GetActualScalingMode(scalingMode);
      System.Drawing.Size size = logicalImage.Size;
      System.Drawing.Size deviceUnits = this.LogicalToDeviceUnits(size);
      if (scalingMode1 == ImageScalingMode.MixedNearestNeighborHighQualityBicubic)
      {
        System.Drawing.Size prescaledImageSize = this.GetPrescaledImageSize(size);
        if (prescaledImageSize == size)
          scalingMode1 = ImageScalingMode.HighQualityBicubic;
        else if (prescaledImageSize == deviceUnits)
          scalingMode1 = ImageScalingMode.NearestNeighbor;
        else if (prescaledImageSize == System.Drawing.Size.Empty)
        {
          scalingMode1 = ImageScalingMode.HighQualityBilinear;
        }
        else
        {
          Image image = this.ScaleLogicalImageForDeviceSize(logicalImage, prescaledImageSize, backgroundColor, ImageScalingMode.NearestNeighbor);
          scalingMode1 = ImageScalingMode.HighQualityBicubic;
          logicalImage = image;
        }
      }
      return this.ScaleLogicalImageForDeviceSize(logicalImage, deviceUnits, backgroundColor, scalingMode1);
    }

    private Image ScaleLogicalImageForDeviceSize(
      Image logicalImage,
      System.Drawing.Size deviceImageSize,
      System.Drawing.Color backgroundColor,
      ImageScalingMode scalingMode)
    {
      DpiHelper.Validate.IsNotNull((object) logicalImage, nameof (logicalImage));
      InterpolationMode interpolationMode = this.GetInterpolationMode(scalingMode);
      System.Drawing.Imaging.PixelFormat pixelFormat = logicalImage.PixelFormat;
      System.Drawing.Color clrMagenta = System.Drawing.Color.FromArgb((int) byte.MaxValue, 0, (int) byte.MaxValue);
      System.Drawing.Color clrNearGreen = System.Drawing.Color.FromArgb(0, 254, 0);
      System.Drawing.Color clrTransparentHalo = System.Drawing.Color.FromArgb(0, 246, 246, 246);
      System.Drawing.Color clrActualBackground = backgroundColor;
      Bitmap image1 = logicalImage as Bitmap;
      if (scalingMode != ImageScalingMode.NearestNeighbor && image1 != null)
      {
        if (pixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
        {
          Rectangle rect = new Rectangle(0, 0, logicalImage.Width, logicalImage.Height);
          logicalImage = (Image) image1.Clone(rect, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
          image1 = logicalImage as Bitmap;
          if (backgroundColor != System.Drawing.Color.Transparent && backgroundColor.A != byte.MaxValue)
            backgroundColor = System.Drawing.Color.FromArgb((int) byte.MaxValue, backgroundColor);
        }
        this.ProcessBitmapPixels(image1, (DpiHelper.PixelProcessor) ((ref byte alpha, ref byte red, ref byte green, ref byte blue) =>
        {
          if (backgroundColor != System.Drawing.Color.Transparent)
          {
            if ((int) alpha != (int) backgroundColor.A || (int) red != (int) backgroundColor.R || (int) green != (int) backgroundColor.G || (int) blue != (int) backgroundColor.B)
              return;
            alpha = clrTransparentHalo.A;
            red = clrTransparentHalo.R;
            green = clrTransparentHalo.G;
            blue = clrTransparentHalo.B;
            clrActualBackground = backgroundColor;
          }
          else if ((int) alpha == (int) clrMagenta.A && (int) red == (int) clrMagenta.R && (int) green == (int) clrMagenta.G && (int) blue == (int) clrMagenta.B)
          {
            alpha = clrTransparentHalo.A;
            red = clrTransparentHalo.R;
            green = clrTransparentHalo.G;
            blue = clrTransparentHalo.B;
            clrActualBackground = clrMagenta;
          }
          else
          {
            if ((int) alpha != (int) clrNearGreen.A || (int) red != (int) clrNearGreen.R || (int) green != (int) clrNearGreen.G || (int) blue != (int) clrNearGreen.B)
              return;
            alpha = clrTransparentHalo.A;
            red = clrTransparentHalo.R;
            green = clrTransparentHalo.G;
            blue = clrTransparentHalo.B;
            clrActualBackground = clrNearGreen;
          }
        }));
        if (clrActualBackground == System.Drawing.Color.Transparent && pixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
          clrActualBackground = !(backgroundColor != System.Drawing.Color.Transparent) ? clrMagenta : backgroundColor;
      }
      Image image2;
      switch (logicalImage)
      {
        case Bitmap _:
          image2 = (Image) new Bitmap(deviceImageSize.Width, deviceImageSize.Height, logicalImage.PixelFormat);
          break;
        case Metafile _:
          IntPtr dc = Microsoft.TeamFoundation.Common.Internal.NativeMethods.GetDC(IntPtr.Zero);
          try
          {
            image2 = (Image) new Metafile(dc, EmfType.EmfPlusDual);
            break;
          }
          finally
          {
            Microsoft.TeamFoundation.Common.Internal.NativeMethods.ReleaseDC(IntPtr.Zero, dc);
          }
        default:
          throw new ArgumentException("Unsupported image type for High DPI conversion", nameof (logicalImage));
      }
      using (Graphics graphics = Graphics.FromImage(image2))
      {
        graphics.InterpolationMode = interpolationMode;
        graphics.Clear(backgroundColor);
        RectangleF srcRect = new RectangleF(0.0f, 0.0f, (float) logicalImage.Size.Width, (float) logicalImage.Size.Height);
        srcRect.Offset(-0.5f, -0.5f);
        RectangleF destRect = new RectangleF(0.0f, 0.0f, (float) deviceImageSize.Width, (float) deviceImageSize.Height);
        if (scalingMode == ImageScalingMode.BorderOnly)
        {
          destRect = new RectangleF(0.0f, 0.0f, srcRect.Width, srcRect.Height);
          destRect.Offset((float) (((double) deviceImageSize.Width - (double) srcRect.Width) / 2.0), (float) (((double) deviceImageSize.Height - (double) srcRect.Height) / 2.0));
        }
        graphics.DrawImage(logicalImage, destRect, srcRect, GraphicsUnit.Pixel);
      }
      Bitmap image3 = image2 as Bitmap;
      if (scalingMode != ImageScalingMode.NearestNeighbor && image3 != null)
      {
        this.ProcessBitmapPixels(image3, (DpiHelper.PixelProcessor) ((ref byte alpha, ref byte red, ref byte green, ref byte blue) =>
        {
          if (alpha == byte.MaxValue)
            return;
          alpha = clrActualBackground.A;
          red = clrActualBackground.R;
          green = clrActualBackground.G;
          blue = clrActualBackground.B;
        }));
        if (pixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
        {
          Rectangle rect = new Rectangle(0, 0, image2.Width, image2.Height);
          image2 = (Image) image3.Clone(rect, pixelFormat);
        }
      }
      return image2;
    }

    public void LogicalToDeviceUnits(
      ref Bitmap imageStrip,
      System.Drawing.Size logicalImageSize,
      ImageScalingMode scalingMode = ImageScalingMode.Default)
    {
      this.LogicalToDeviceUnits(ref imageStrip, logicalImageSize, System.Drawing.Color.Transparent, scalingMode);
    }

    public void LogicalToDeviceUnits(
      ref Bitmap imageStrip,
      System.Drawing.Size logicalImageSize,
      System.Drawing.Color backgroundColor,
      ImageScalingMode scalingMode = ImageScalingMode.Default)
    {
      DpiHelper.Validate.IsNotNull((object) imageStrip, nameof (imageStrip));
      if (!this.IsScalingRequired)
        return;
      Bitmap fromLogicalImage = this.CreateDeviceFromLogicalImage(imageStrip, logicalImageSize, backgroundColor, scalingMode);
      imageStrip.Dispose();
      imageStrip = fromLogicalImage;
    }

    public Bitmap CreateDeviceFromLogicalImage(
      Bitmap logicalBitmapStrip,
      System.Drawing.Size logicalImageSize,
      ImageScalingMode scalingMode = ImageScalingMode.Default)
    {
      return this.CreateDeviceFromLogicalImage(logicalBitmapStrip, logicalImageSize, System.Drawing.Color.Transparent, scalingMode);
    }

    public Bitmap CreateDeviceFromLogicalImage(
      Bitmap logicalBitmapStrip,
      System.Drawing.Size logicalImageSize,
      System.Drawing.Color backgroundColor,
      ImageScalingMode scalingMode = ImageScalingMode.Default)
    {
      DpiHelper.Validate.IsNotNull((object) logicalBitmapStrip, nameof (logicalBitmapStrip));
      DpiHelper.Validate.IsNotNull((object) logicalImageSize, nameof (logicalImageSize));
      if (logicalImageSize.Width == 0 || logicalBitmapStrip.Height % logicalImageSize.Width != 0 || logicalImageSize.Height != logicalBitmapStrip.Height)
        throw new ArgumentException("logicalImageSize not matching the logicalBitmap size");
      int num = logicalBitmapStrip.Width / logicalImageSize.Width;
      int deviceUnitsX = this.LogicalToDeviceUnitsX(logicalImageSize.Width);
      int deviceUnitsY = this.LogicalToDeviceUnitsY(logicalImageSize.Height);
      Bitmap fromLogicalImage = new Bitmap(num * deviceUnitsX, deviceUnitsY, logicalBitmapStrip.PixelFormat);
      using (Graphics graphics1 = Graphics.FromImage((Image) fromLogicalImage))
      {
        graphics1.InterpolationMode = InterpolationMode.NearestNeighbor;
        for (int index = 0; index < num; ++index)
        {
          RectangleF srcRect = new RectangleF((float) (index * logicalImageSize.Width), 0.0f, (float) logicalImageSize.Width, (float) logicalImageSize.Height);
          srcRect.Offset(-0.5f, -0.5f);
          RectangleF destRect = new RectangleF(0.0f, 0.0f, (float) logicalImageSize.Width, (float) logicalImageSize.Height);
          Bitmap bitmapImage = new Bitmap(logicalImageSize.Width, logicalImageSize.Height, logicalBitmapStrip.PixelFormat);
          using (Graphics graphics2 = Graphics.FromImage((Image) bitmapImage))
          {
            graphics2.InterpolationMode = InterpolationMode.NearestNeighbor;
            graphics2.DrawImage((Image) logicalBitmapStrip, destRect, srcRect, GraphicsUnit.Pixel);
          }
          this.LogicalToDeviceUnits(ref bitmapImage, backgroundColor, scalingMode);
          srcRect = new RectangleF(0.0f, 0.0f, (float) deviceUnitsX, (float) deviceUnitsY);
          srcRect.Offset(-0.5f, -0.5f);
          destRect = new RectangleF((float) (index * deviceUnitsX), 0.0f, (float) deviceUnitsX, (float) deviceUnitsY);
          graphics1.DrawImage((Image) bitmapImage, destRect, srcRect, GraphicsUnit.Pixel);
        }
      }
      return fromLogicalImage;
    }

    public void LogicalToDeviceUnits(ref Icon icon, ImageScalingMode scalingMode = ImageScalingMode.Default)
    {
      DpiHelper.Validate.IsNotNull((object) icon, nameof (icon));
      if (!this.IsScalingRequired)
        return;
      Icon fromLogicalImage = this.CreateDeviceFromLogicalImage(icon, scalingMode);
      icon.Dispose();
      icon = fromLogicalImage;
    }

    public Icon CreateDeviceFromLogicalImage(Icon logicalIcon, ImageScalingMode scalingMode = ImageScalingMode.Default)
    {
      DpiHelper.Validate.IsNotNull((object) logicalIcon, nameof (logicalIcon));
      System.Drawing.Size deviceUnits = this.LogicalToDeviceUnits(logicalIcon.Size);
      Icon fromLogicalImage = new Icon(logicalIcon, deviceUnits);
      if (fromLogicalImage.Size.Width != deviceUnits.Width && fromLogicalImage.Size.Width != 0)
      {
        IntPtr hicon = ((Bitmap) this.CreateDeviceFromLogicalImage((Image) fromLogicalImage.ToBitmap(), System.Drawing.Color.Transparent, scalingMode)).GetHicon();
        fromLogicalImage = Icon.FromHandle(hicon).Clone() as Icon;
        Microsoft.TeamFoundation.Common.Internal.NativeMethods.DestroyIcon(hicon);
      }
      return fromLogicalImage;
    }

    public void LogicalToDeviceUnits(ref ImageList imageList, ImageScalingMode scalingMode = ImageScalingMode.Default) => this.LogicalToDeviceUnits(ref imageList, System.Drawing.Color.Transparent, scalingMode);

    public void LogicalToDeviceUnits(
      ref ImageList imageList,
      System.Drawing.Color backgroundColor,
      ImageScalingMode scalingMode = ImageScalingMode.Default)
    {
      DpiHelper.Validate.IsNotNull((object) imageList, nameof (imageList));
      if (!this.IsScalingRequired)
        return;
      ImageList fromLogicalImage = this.CreateDeviceFromLogicalImage(imageList, backgroundColor, scalingMode);
      imageList.Dispose();
      imageList = fromLogicalImage;
    }

    public ImageList CreateDeviceFromLogicalImage(
      ImageList logicalImageList,
      ImageScalingMode scalingMode = ImageScalingMode.Default)
    {
      return this.CreateDeviceFromLogicalImage(logicalImageList, System.Drawing.Color.Transparent, scalingMode);
    }

    public ImageList CreateDeviceFromLogicalImage(
      ImageList logicalImageList,
      System.Drawing.Color backgroundColor,
      ImageScalingMode scalingMode = ImageScalingMode.Default)
    {
      DpiHelper.Validate.IsNotNull((object) logicalImageList, nameof (logicalImageList));
      ImageList fromLogicalImage1 = new ImageList();
      fromLogicalImage1.Site = logicalImageList.Site;
      fromLogicalImage1.Tag = logicalImageList.Tag;
      fromLogicalImage1.ColorDepth = logicalImageList.ColorDepth;
      fromLogicalImage1.TransparentColor = logicalImageList.TransparentColor;
      fromLogicalImage1.ImageSize = this.LogicalToDeviceUnits(logicalImageList.ImageSize);
      for (int index = 0; index < logicalImageList.Images.Count; ++index)
      {
        Image fromLogicalImage2 = this.CreateDeviceFromLogicalImage(logicalImageList.Images[index], backgroundColor, scalingMode);
        fromLogicalImage1.Images.Add(fromLogicalImage2);
      }
      foreach (string key in logicalImageList.Images.Keys)
      {
        int index = logicalImageList.Images.IndexOfKey(key);
        if (index != -1)
          fromLogicalImage1.Images.SetKeyName(index, key);
      }
      return fromLogicalImage1;
    }

    private static class Validate
    {
      public static void IsNotNull(object arg, string argname)
      {
        if (arg == null)
          throw new ArgumentNullException("Argument {0} null.", argname);
      }
    }

    private delegate void PixelProcessor(
      ref byte alpha,
      ref byte red,
      ref byte green,
      ref byte blue);
  }
}
