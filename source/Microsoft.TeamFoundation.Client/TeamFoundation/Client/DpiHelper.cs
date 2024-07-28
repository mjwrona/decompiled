// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.DpiHelper
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace Microsoft.TeamFoundation.Client
{
  public static class DpiHelper
  {
    public static DpiHelper.DpiHelperImplementation Instance { get; private set; }

    static DpiHelper() => DpiHelper.Instance = new DpiHelper.DpiHelperImplementation();

    public static ImageScalingMode ImageScalingMode => DpiHelper.Instance.ImageScalingMode;

    public static BitmapScalingMode BitmapScalingMode => DpiHelper.Instance.BitmapScalingMode;

    public static bool UsePreScaledImages => DpiHelper.Instance.UsePreScaledImages;

    public static MatrixTransform TransformFromDevice => DpiHelper.Instance.TransformFromDevice;

    public static MatrixTransform TransformToDevice => DpiHelper.Instance.TransformToDevice;

    public static double DeviceDpiX => DpiHelper.Instance.DeviceDpiX;

    public static double DeviceDpiY => DpiHelper.Instance.DeviceDpiY;

    public static double LogicalDpiX => DpiHelper.Instance.LogicalDpiX;

    public static double LogicalDpiY => DpiHelper.Instance.LogicalDpiY;

    public static bool IsScalingRequired => DpiHelper.Instance.IsScalingRequired;

    public static double DeviceToLogicalUnitsScalingFactorX => DpiHelper.Instance.DeviceToLogicalUnitsScalingFactorX;

    public static double DeviceToLogicalUnitsScalingFactorY => DpiHelper.Instance.DeviceToLogicalUnitsScalingFactorY;

    public static double LogicalToDeviceUnitsScalingFactorX => DpiHelper.Instance.LogicalToDeviceUnitsScalingFactorX;

    public static double LogicalToDeviceUnitsScalingFactorY => DpiHelper.Instance.LogicalToDeviceUnitsScalingFactorY;

    public static double LogicalToDeviceUnitsX(double value) => DpiHelper.Instance.LogicalToDeviceUnitsX(value);

    public static double LogicalToDeviceUnitsY(double value) => DpiHelper.Instance.LogicalToDeviceUnitsY(value);

    public static double DeviceToLogicalUnitsX(double value) => DpiHelper.Instance.DeviceToLogicalUnitsX(value);

    public static double DeviceToLogicalUnitsY(double value) => DpiHelper.Instance.DeviceToLogicalUnitsY(value);

    public static float LogicalToDeviceUnitsX(float value) => DpiHelper.Instance.LogicalToDeviceUnitsX(value);

    public static float LogicalToDeviceUnitsY(float value) => DpiHelper.Instance.LogicalToDeviceUnitsY(value);

    public static int LogicalToDeviceUnitsX(int value) => DpiHelper.Instance.LogicalToDeviceUnitsX(value);

    public static int LogicalToDeviceUnitsY(int value) => DpiHelper.Instance.LogicalToDeviceUnitsY(value);

    public static float DeviceToLogicalUnitsX(float value) => DpiHelper.Instance.DeviceToLogicalUnitsX(value);

    public static float DeviceToLogicalUnitsY(float value) => DpiHelper.Instance.DeviceToLogicalUnitsY(value);

    public static int DeviceToLogicalUnitsX(int value) => DpiHelper.Instance.DeviceToLogicalUnitsX(value);

    public static int DeviceToLogicalUnitsY(int value) => DpiHelper.Instance.DeviceToLogicalUnitsY(value);

    public static double RoundToDeviceUnitsX(double value) => DpiHelper.Instance.RoundToDeviceUnitsX(value);

    public static double RoundToDeviceUnitsY(double value) => DpiHelper.Instance.RoundToDeviceUnitsY(value);

    public static Padding LogicalToDeviceUnits(this Padding logicalPadding) => DpiHelper.Instance.LogicalToDeviceUnits(logicalPadding);

    public static System.Windows.Point LogicalToDeviceUnits(this System.Windows.Point logicalPoint) => DpiHelper.Instance.LogicalToDeviceUnits(logicalPoint);

    public static Rect LogicalToDeviceUnits(this Rect logicalRect) => DpiHelper.Instance.LogicalToDeviceUnits(logicalRect);

    public static System.Windows.Size LogicalToDeviceUnits(this System.Windows.Size logicalSize) => DpiHelper.Instance.LogicalToDeviceUnits(logicalSize);

    public static Thickness LogicalToDeviceUnits(this Thickness logicalThickness) => DpiHelper.Instance.LogicalToDeviceUnits(logicalThickness);

    public static System.Windows.Point DeviceToLogicalUnits(this System.Windows.Point devicePoint) => DpiHelper.Instance.DeviceToLogicalUnits(devicePoint);

    public static Rect DeviceToLogicalUnits(this Rect deviceRect) => DpiHelper.Instance.DeviceToLogicalUnits(deviceRect);

    public static System.Windows.Size DeviceToLogicalUnits(this System.Windows.Size deviceSize) => DpiHelper.Instance.DeviceToLogicalUnits(deviceSize);

    public static Thickness DeviceToLogicalUnits(this Thickness deviceThickness) => DpiHelper.Instance.DeviceToLogicalUnits(deviceThickness);

    public static void SetDeviceLeft(this Window window, double deviceLeft) => DpiHelper.Instance.SetDeviceLeft(ref window, deviceLeft);

    public static double GetDeviceLeft(this Window window) => DpiHelper.Instance.GetDeviceLeft(window);

    public static void SetDeviceTop(this Window window, double deviceTop) => DpiHelper.Instance.SetDeviceTop(ref window, deviceTop);

    public static double GetDeviceTop(this Window window) => DpiHelper.Instance.GetDeviceTop(window);

    public static void SetDeviceWidth(this Window window, double deviceWidth) => DpiHelper.Instance.SetDeviceWidth(ref window, deviceWidth);

    public static double GetDeviceWidth(this Window window) => DpiHelper.Instance.GetDeviceWidth(window);

    public static void SetDeviceHeight(this Window window, double deviceHeight) => DpiHelper.Instance.SetDeviceHeight(ref window, deviceHeight);

    public static double GetDeviceHeight(this Window window) => DpiHelper.Instance.GetDeviceHeight(window);

    public static Rect GetDeviceRect(this Window window) => DpiHelper.Instance.GetDeviceRect(window);

    public static System.Windows.Size GetDeviceActualSize(this FrameworkElement element) => DpiHelper.Instance.GetDeviceActualSize(element);

    public static System.Drawing.Point LogicalToDeviceUnits(this System.Drawing.Point logicalPoint) => DpiHelper.Instance.LogicalToDeviceUnits(logicalPoint);

    public static System.Drawing.Size LogicalToDeviceUnits(this System.Drawing.Size logicalSize) => DpiHelper.Instance.LogicalToDeviceUnits(logicalSize);

    public static Rectangle LogicalToDeviceUnits(this Rectangle logicalRect) => DpiHelper.Instance.LogicalToDeviceUnits(logicalRect);

    public static PointF LogicalToDeviceUnits(this PointF logicalPoint) => DpiHelper.Instance.LogicalToDeviceUnits(logicalPoint);

    public static SizeF LogicalToDeviceUnits(this SizeF logicalSize) => DpiHelper.Instance.LogicalToDeviceUnits(logicalSize);

    public static RectangleF LogicalToDeviceUnits(this RectangleF logicalRect) => DpiHelper.Instance.LogicalToDeviceUnits(logicalRect);

    public static void LogicalToDeviceUnits(ref Bitmap bitmapImage, ImageScalingMode scalingMode = ImageScalingMode.Default) => DpiHelper.Instance.LogicalToDeviceUnits(ref bitmapImage, scalingMode);

    public static void LogicalToDeviceUnits(
      ref Bitmap bitmapImage,
      System.Drawing.Color backgroundColor,
      ImageScalingMode scalingMode = ImageScalingMode.Default)
    {
      DpiHelper.Instance.LogicalToDeviceUnits(ref bitmapImage, backgroundColor, scalingMode);
    }

    public static void LogicalToDeviceUnits(ref Image image, ImageScalingMode scalingMode = ImageScalingMode.Default) => DpiHelper.Instance.LogicalToDeviceUnits(ref image, scalingMode);

    public static void LogicalToDeviceUnits(
      ref Image image,
      System.Drawing.Color backgroundColor,
      ImageScalingMode scalingMode = ImageScalingMode.Default)
    {
      DpiHelper.Instance.LogicalToDeviceUnits(ref image, backgroundColor, scalingMode);
    }

    public static int DpiScalePercentX => DpiHelper.Instance.DpiScalePercentX;

    public static int DpiScalePercentY => DpiHelper.Instance.DpiScalePercentY;

    public static double PreScaledImageLayoutTransformScaleX => DpiHelper.Instance.PreScaledImageLayoutTransformScaleX;

    public static double PreScaledImageLayoutTransformScaleY => DpiHelper.Instance.PreScaledImageLayoutTransformScaleY;

    public static Image CreateDeviceFromLogicalImage(
      this Image logicalImage,
      ImageScalingMode scalingMode = ImageScalingMode.Default)
    {
      return DpiHelper.Instance.CreateDeviceFromLogicalImage(logicalImage, scalingMode);
    }

    public static ImageSource ScaleLogicalImageForDeviceSize(
      ImageSource image,
      System.Windows.Size deviceImageSize,
      BitmapScalingMode scalingMode)
    {
      return DpiHelper.Instance.ScaleLogicalImageForDeviceSize(image, deviceImageSize, scalingMode);
    }

    public static Image CreateDeviceFromLogicalImage(
      this Image logicalImage,
      System.Drawing.Color backgroundColor,
      ImageScalingMode scalingMode = ImageScalingMode.Default)
    {
      return DpiHelper.Instance.CreateDeviceFromLogicalImage(logicalImage, backgroundColor, scalingMode);
    }

    public static void LogicalToDeviceUnits(
      ref Bitmap imageStrip,
      System.Drawing.Size logicalImageSize,
      ImageScalingMode scalingMode = ImageScalingMode.Default)
    {
      DpiHelper.Instance.LogicalToDeviceUnits(ref imageStrip, logicalImageSize, scalingMode);
    }

    public static void LogicalToDeviceUnits(
      ref Bitmap imageStrip,
      System.Drawing.Size logicalImageSize,
      System.Drawing.Color backgroundColor,
      ImageScalingMode scalingMode = ImageScalingMode.Default)
    {
      DpiHelper.Instance.LogicalToDeviceUnits(ref imageStrip, logicalImageSize, backgroundColor, scalingMode);
    }

    public static Bitmap CreateDeviceFromLogicalImage(
      this Bitmap logicalBitmapStrip,
      System.Drawing.Size logicalImageSize,
      ImageScalingMode scalingMode = ImageScalingMode.Default)
    {
      return DpiHelper.Instance.CreateDeviceFromLogicalImage(logicalBitmapStrip, logicalImageSize, scalingMode);
    }

    public static Bitmap CreateDeviceFromLogicalImage(
      this Bitmap logicalBitmapStrip,
      System.Drawing.Size logicalImageSize,
      System.Drawing.Color backgroundColor,
      ImageScalingMode scalingMode = ImageScalingMode.Default)
    {
      return DpiHelper.Instance.CreateDeviceFromLogicalImage(logicalBitmapStrip, logicalImageSize, backgroundColor, scalingMode);
    }

    public static void LogicalToDeviceUnits(ref Icon icon, ImageScalingMode scalingMode = ImageScalingMode.Default) => DpiHelper.Instance.LogicalToDeviceUnits(ref icon, scalingMode);

    public static Icon CreateDeviceFromLogicalImage(
      this Icon logicalIcon,
      ImageScalingMode scalingMode = ImageScalingMode.Default)
    {
      return DpiHelper.Instance.CreateDeviceFromLogicalImage(logicalIcon, scalingMode);
    }

    public static void LogicalToDeviceUnits(ref ImageList imageList, ImageScalingMode scalingMode = ImageScalingMode.Default) => DpiHelper.Instance.LogicalToDeviceUnits(ref imageList, scalingMode);

    public static void LogicalToDeviceUnits(
      ref ImageList imageList,
      System.Drawing.Color backgroundColor,
      ImageScalingMode scalingMode = ImageScalingMode.Default)
    {
      DpiHelper.Instance.LogicalToDeviceUnits(ref imageList, backgroundColor, scalingMode);
    }

    public static ImageList CreateDeviceFromLogicalImage(
      this ImageList logicalImageList,
      ImageScalingMode scalingMode = ImageScalingMode.Default)
    {
      return DpiHelper.Instance.CreateDeviceFromLogicalImage(logicalImageList, scalingMode);
    }

    public static ImageList CreateDeviceFromLogicalImage(
      this ImageList logicalImageList,
      System.Drawing.Color backgroundColor,
      ImageScalingMode scalingMode = ImageScalingMode.Default)
    {
      return DpiHelper.Instance.CreateDeviceFromLogicalImage(logicalImageList, backgroundColor, scalingMode);
    }

    public class DpiHelperImplementation : Microsoft.TeamFoundation.Client.Dpi.DpiHelper
    {
      public DpiHelperImplementation()
        : base(96.0)
      {
      }
    }
  }
}
