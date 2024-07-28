// Decompiled with JetBrains decompiler
// Type: WebGrease.ImageAssemble.ImageAssembleBase
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using WebGrease.Css.ImageAssemblyAnalysis;
using WebGrease.Extensions;

namespace WebGrease.ImageAssemble
{
  internal abstract class ImageAssembleBase
  {
    private readonly IWebGreaseContext context;

    public ImageAssembleBase(IWebGreaseContext context) => this.context = context;

    internal abstract ImageType Type { get; }

    internal abstract string DefaultExtension { get; }

    internal string AssembleFileName { get; set; }

    internal SpritePackingType PackingType { get; set; }

    internal ImageMap ImageXmlMap { get; set; }

    internal int PaddingBetweenImages { get; set; }

    internal string OptimizerToolCommand { get; set; }

    protected abstract ImageFormat Format { get; }

    internal virtual bool Assemble(List<BitmapContainer> inputImages)
    {
      Bitmap bitmap1 = (Bitmap) null;
      try
      {
        if (inputImages.HasAtLeast<BitmapContainer>(2))
        {
          switch (this.PackingType)
          {
            case SpritePackingType.Vertical:
              bitmap1 = this.PackVertical(inputImages, true, new PixelFormat?());
              break;
            case SpritePackingType.Horizontal:
              bitmap1 = this.PackHorizontal(inputImages, true, new PixelFormat?());
              break;
            default:
              bitmap1 = this.PackVertical(inputImages, true, new PixelFormat?());
              break;
          }
          if (bitmap1 != null)
          {
            this.SaveAndHashImage(bitmap1, bitmap1.Width, bitmap1.Height);
            return true;
          }
        }
        else if (inputImages.Any<BitmapContainer>())
        {
          BitmapContainer image = inputImages.First<BitmapContainer>();
          this.ImageXmlMap.AppendToXml(image.InputImage.AbsoluteImagePath, this.AssembleFileName, image.Width, image.Height, 0, 0, "passthrough", true, new ImagePosition?(image.InputImage.Position));
          image.BitmapAction((Action<Bitmap>) (bitmap => this.SaveAndHashImage(image.Bitmap, image.Width, image.Height)));
          return true;
        }
      }
      catch (OutOfMemoryException ex)
      {
        this.context.Log.Error((Exception) ex);
        throw new ImageAssembleException(ImageAssembleStrings.ImageLoadOutofMemoryExceptionMessage, (Exception) ex);
      }
      catch (Exception ex1)
      {
        this.context.Log.Error(ex1);
        try
        {
          Safe.FileLock((FileSystemInfo) new FileInfo(this.AssembleFileName), (Action) (() =>
          {
            if (!File.Exists(this.AssembleFileName))
              return;
            File.Delete(this.AssembleFileName);
          }));
        }
        catch (Exception ex2)
        {
        }
        throw;
      }
      finally
      {
        bitmap1?.Dispose();
      }
      return false;
    }

    private void SaveAndHashImage(Bitmap bitmap, int width, int height)
    {
      this.AssembleFileName = this.HashImage(this.context.GetBitmapHash(bitmap, this.Format));
      FileInfo targetFileInfo = new FileInfo(this.AssembleFileName);
      Safe.FileLock((FileSystemInfo) targetFileInfo, (Action) (() =>
      {
        if (targetFileInfo.Exists)
          return;
        this.SaveImage(bitmap);
      }));
      this.ImageXmlMap.UpdateSize(this.AssembleFileName, width, height);
    }

    protected virtual void SaveImage(Bitmap newImage)
    {
      try
      {
        if (File.Exists(this.AssembleFileName))
          return;
        newImage.Save(this.AssembleFileName, this.Format);
      }
      catch (ExternalException ex)
      {
        throw new ImageAssembleException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, ImageAssembleStrings.ImageSaveExternalExceptionMessage, new object[1]
        {
          (object) this.AssembleFileName
        }), (Exception) ex);
      }
    }

    protected void OptimizeImage()
    {
      if (string.IsNullOrEmpty(this.OptimizerToolCommand))
        return;
      this.OptimizerToolCommand = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + this.OptimizerToolCommand;
      string str1 = ".exe";
      string str2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, this.OptimizerToolCommand, new object[1]
      {
        (object) this.AssembleFileName
      });
      int num = str2.IndexOf(str1, StringComparison.OrdinalIgnoreCase);
      string str3 = str2.Substring(0, num + str1.Length + 1);
      Trace.WriteLine("Image Optimization Executable - " + str3);
      if (!File.Exists(str3))
        throw new FileNotFoundException("Could not locate the image optimization executable.", str3);
      Process.Start(new ProcessStartInfo(str3)
      {
        CreateNoWindow = true,
        Arguments = str2.Replace(str3, string.Empty),
        UseShellExecute = false
      }).WaitForExit();
    }

    protected string HashImage(string hash)
    {
      FileInfo fileInfo = new FileInfo(this.AssembleFileName);
      string str1 = hash + fileInfo.Extension;
      string str2 = Path.Combine(fileInfo.DirectoryName, str1.Substring(0, 2));
      Directory.CreateDirectory(str2);
      string newName = Path.Combine(str2, str1.Remove(0, 2));
      if (!this.ImageXmlMap.UpdateAssembledImageName(this.AssembleFileName, newName))
        throw new ImageAssembleException((string) null, this.AssembleFileName, "Operation failed while replacing assembled image name: '" + this.AssembleFileName + "' with hashed name.");
      return newName;
    }

    protected Bitmap PackHorizontal(
      List<BitmapContainer> originalBitmaps,
      bool useLogging,
      PixelFormat? pixelFormat)
    {
      int height = originalBitmaps.Max<BitmapContainer>((Func<BitmapContainer, int>) (c => c.Height));
      int width = originalBitmaps.Sum<BitmapContainer>((Func<BitmapContainer, int>) (c => c.Width)) + originalBitmaps.Count * this.PaddingBetweenImages;
      Bitmap bitmap1 = pixelFormat.HasValue ? new Bitmap(width, height, pixelFormat.Value) : new Bitmap(width, height);
      IOrderedEnumerable<BitmapContainer> orderedEnumerable = originalBitmaps.OrderByDescending<BitmapContainer, int>((Func<BitmapContainer, int>) (entry => entry.Height));
      using (Graphics graphics = Graphics.FromImage((Image) bitmap1))
      {
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.SmoothingMode = SmoothingMode.HighQuality;
        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        graphics.CompositingQuality = CompositingQuality.HighQuality;
        int xpoint = 0;
        bool addOutputNode = true;
        foreach (BitmapContainer bitmapContainer in (IEnumerable<BitmapContainer>) orderedEnumerable)
        {
          BitmapContainer entry = bitmapContainer;
          entry.BitmapAction((Action<Bitmap>) (bitmap => graphics.DrawImage((Image) bitmap, new Rectangle(xpoint, 0, entry.Width, entry.Height))));
          if (useLogging)
          {
            this.ImageXmlMap.AppendToXml(entry.InputImage.AbsoluteImagePath, this.AssembleFileName, entry.Width, entry.Height, xpoint * -1, 0, (string) null, addOutputNode, new ImagePosition?(entry.InputImage.Position));
            addOutputNode = false;
            foreach (string duplicateImagePath in (IEnumerable<string>) entry.InputImage.DuplicateImagePaths)
              this.ImageXmlMap.AppendToXml(duplicateImagePath, this.AssembleFileName, entry.Width, entry.Height, xpoint * -1, 0, "duplicate", addOutputNode, new ImagePosition?(entry.InputImage.Position));
          }
          xpoint += entry.Width + this.PaddingBetweenImages;
        }
      }
      return bitmap1;
    }

    protected Bitmap PackVertical(
      List<BitmapContainer> originalBitmaps,
      bool useLogging,
      PixelFormat? pixelFormat)
    {
      int width = originalBitmaps.Max<BitmapContainer>((Func<BitmapContainer, int>) (c => c.Width));
      int height = originalBitmaps.Sum<BitmapContainer>((Func<BitmapContainer, int>) (c => c.Height)) + originalBitmaps.Count * this.PaddingBetweenImages;
      Bitmap bitmap1 = pixelFormat.HasValue ? new Bitmap(width, height, pixelFormat.Value) : new Bitmap(width, height);
      IOrderedEnumerable<BitmapContainer> orderedEnumerable = originalBitmaps.OrderByDescending<BitmapContainer, int>((Func<BitmapContainer, int>) (entry => entry.Width));
      using (Graphics graphics = Graphics.FromImage((Image) bitmap1))
      {
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.SmoothingMode = SmoothingMode.HighQuality;
        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        graphics.CompositingQuality = CompositingQuality.HighQuality;
        int ypoint = 0;
        bool addOutputNode = true;
        foreach (BitmapContainer bitmapContainer in (IEnumerable<BitmapContainer>) orderedEnumerable)
        {
          BitmapContainer entry = bitmapContainer;
          int xpoint = 0;
          switch (entry.InputImage.Position)
          {
            case ImagePosition.Right:
              xpoint = bitmap1.Width - entry.Width;
              break;
            case ImagePosition.Center:
              xpoint = (bitmap1.Width - entry.Width + 1) / 2;
              break;
          }
          entry.BitmapAction((Action<Bitmap>) (bitmap => graphics.DrawImage((Image) bitmap, new Rectangle(xpoint, ypoint, entry.Width, entry.Height))));
          if (useLogging)
          {
            this.ImageXmlMap.AppendToXml(entry.InputImage.AbsoluteImagePath, this.AssembleFileName, entry.Width, entry.Height, xpoint * -1, ypoint * -1, (string) null, addOutputNode, new ImagePosition?(entry.InputImage.Position));
            addOutputNode = false;
            foreach (string duplicateImagePath in (IEnumerable<string>) entry.InputImage.DuplicateImagePaths)
              this.ImageXmlMap.AppendToXml(duplicateImagePath, this.AssembleFileName, entry.Width, entry.Height, xpoint * -1, ypoint * -1, "duplicate", addOutputNode, new ImagePosition?(entry.InputImage.Position));
          }
          ypoint += entry.Height + this.PaddingBetweenImages;
        }
      }
      return bitmap1;
    }
  }
}
