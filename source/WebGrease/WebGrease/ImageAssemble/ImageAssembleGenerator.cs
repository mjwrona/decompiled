// Decompiled with JetBrains decompiler
// Type: WebGrease.ImageAssemble.ImageAssembleGenerator
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using WebGrease.Css.ImageAssemblyAnalysis;
using WebGrease.Css.ImageAssemblyAnalysis.LogModel;
using WebGrease.Extensions;

namespace WebGrease.ImageAssemble
{
  internal static class ImageAssembleGenerator
  {
    private const int MaxRetryCount = 4;
    private const int RetrySleepMilliseconds = 500;
    private const int DefaultPadding = 50;

    internal static ImageMap AssembleImages(
      ReadOnlyCollection<InputImage> inputImages,
      SpritePackingType packingType,
      string assembleFileFolder,
      string pngOptimizerToolCommand,
      bool dedup,
      IWebGreaseContext context,
      int? imagePadding = null,
      ImageAssemblyAnalysisLog imageAssemblyAnalysisLog = null,
      ImageType? forcedImageType = null)
    {
      return ImageAssembleGenerator.AssembleImages(inputImages, packingType, assembleFileFolder, (string) null, pngOptimizerToolCommand, dedup, context, imagePadding, imageAssemblyAnalysisLog, forcedImageType);
    }

    internal static ImageMap AssembleImages(
      ReadOnlyCollection<InputImage> inputImages,
      SpritePackingType packingType,
      string assembleFileFolder,
      string mapFileName,
      string pngOptimizerToolCommand,
      bool dedup,
      IWebGreaseContext context,
      int? imagePadding = null,
      ImageAssemblyAnalysisLog imageAssemblyAnalysisLog = null,
      ImageType? forcedImageType = null)
    {
      ReadOnlyCollection<InputImage> inputImagesDeduped = dedup ? ImageAssembleGenerator.DedupImages(inputImages, context) : inputImages;
      ImageMap xmlMap = new ImageMap(mapFileName);
      Safe.LockFiles((IEnumerable<FileInfo>) inputImages.Select<InputImage, FileInfo>((Func<InputImage, FileInfo>) (ii => new FileInfo(ii.AbsoluteImagePath))).ToArray<FileInfo>(), (Action) (() =>
      {
        Dictionary<ImageType, List<BitmapContainer>> dictionary = ImageAssembleGenerator.SeparateByImageType((IEnumerable<InputImage>) inputImagesDeduped, forcedImageType);
        int num = imagePadding ?? 50;
        IEnumerable<ImageAssembleBase> imageAssembleBases = ImageAssembleGenerator.RegisterAvailableAssemblers(context);
        List<BitmapContainer> bitmapContainerList1 = (List<BitmapContainer>) null;
        foreach (ImageAssembleBase imageAssembleBase in imageAssembleBases)
        {
          bool flag = false;
          try
          {
            imageAssembleBase.PackingType = packingType;
            imageAssembleBase.ImageXmlMap = xmlMap;
            xmlMap.AppendPadding(num.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            imageAssembleBase.PaddingBetweenImages = num;
            imageAssembleBase.OptimizerToolCommand = pngOptimizerToolCommand;
            bitmapContainerList1 = dictionary[imageAssembleBase.Type];
            if (bitmapContainerList1.Any<BitmapContainer>())
            {
              imageAssembleBase.AssembleFileName = ImageAssembleGenerator.GenerateAssembleFileName(bitmapContainerList1.Select<BitmapContainer, InputImage>((Func<BitmapContainer, InputImage>) (s => s.InputImage)), assembleFileFolder) + imageAssembleBase.DefaultExtension;
              flag = imageAssembleBase.Assemble(bitmapContainerList1);
            }
          }
          finally
          {
            if (flag)
            {
              foreach (BitmapContainer bitmapContainer in bitmapContainerList1)
              {
                if (bitmapContainer.Bitmap != null)
                {
                  imageAssemblyAnalysisLog?.UpdateSpritedImage(imageAssembleBase.Type, bitmapContainer.InputImage.OriginalImagePath, imageAssembleBase.AssembleFileName);
                  context.Cache.CurrentCacheSection.AddSourceDependency(bitmapContainer.InputImage.AbsoluteImagePath);
                  bitmapContainer.Bitmap.Dispose();
                }
              }
            }
          }
        }
        List<BitmapContainer> bitmapContainerList2 = dictionary[ImageType.NotSupported];
        if (bitmapContainerList2 != null && bitmapContainerList2.Count > 0)
        {
          StringBuilder stringBuilder = new StringBuilder("The following files were not assembled because their formats are not supported:");
          foreach (BitmapContainer bitmapContainer in bitmapContainerList2)
            stringBuilder.Append(" " + bitmapContainer.InputImage.OriginalImagePath);
          throw new ImageAssembleException(stringBuilder.ToString());
        }
      }));
      return xmlMap;
    }

    internal static Dictionary<ImageType, List<BitmapContainer>> SeparateByImageType(
      IEnumerable<InputImage> inputImages,
      ImageType? forcedImageType = null)
    {
      Dictionary<ImageType, List<BitmapContainer>> separatedLists = new Dictionary<ImageType, List<BitmapContainer>>();
      foreach (ImageType key in Enum.GetValues(typeof (ImageType)))
        separatedLists[key] = new List<BitmapContainer>();
      foreach (InputImage inputImage1 in inputImages)
      {
        InputImage inputImage = inputImage1;
        BitmapContainer bitmapContainer = new BitmapContainer(inputImage);
        bitmapContainer.BitmapAction((Action<Bitmap>) (b =>
        {
          bitmapContainer.Bitmap = ImageAssembleGenerator.LoadBitmapFromDisk(inputImage.AbsoluteImagePath);
          if (bitmapContainer.Bitmap == null)
            separatedLists[ImageType.NotSupported].Add(bitmapContainer);
          else if (forcedImageType.HasValue)
            separatedLists[forcedImageType.Value].Add(bitmapContainer);
          else if (ImageAssembleGenerator.IsPhoto(bitmapContainer.Bitmap))
            separatedLists[ImageType.Photo].Add(bitmapContainer);
          else if (ImageAssembleGenerator.IsMultiframe(bitmapContainer.Bitmap))
            separatedLists[ImageType.NotSupported].Add(bitmapContainer);
          else if (ImageAssembleGenerator.IsIndexed(bitmapContainer.Bitmap) || ImageAssembleGenerator.IsIndexable(bitmapContainer.Bitmap))
            separatedLists[ImageType.NonphotoIndexed].Add(bitmapContainer);
          else
            separatedLists[ImageType.NonphotoNonindexed].Add(bitmapContainer);
        }));
      }
      return separatedLists;
    }

    private static Bitmap LoadBitmapFromDisk(string absoluteImagePath, int retryCount = 0)
    {
      try
      {
        return Image.FromFile(absoluteImagePath) as Bitmap;
      }
      catch (OutOfMemoryException ex)
      {
        if (retryCount < 4)
        {
          Thread.Sleep(500);
          return ImageAssembleGenerator.LoadBitmapFromDisk(absoluteImagePath, ++retryCount);
        }
        throw;
      }
    }

    private static IEnumerable<ImageAssembleBase> RegisterAvailableAssemblers(
      IWebGreaseContext context)
    {
      return (IEnumerable<ImageAssembleBase>) new List<ImageAssembleBase>()
      {
        (ImageAssembleBase) new NotSupportedAssemble(context),
        (ImageAssembleBase) new PhotoAssemble(context),
        (ImageAssembleBase) new NonphotoNonindexedAssemble(context),
        (ImageAssembleBase) new NonphotoIndexedAssemble(context)
      };
    }

    private static string GenerateAssembleFileName(
      IEnumerable<InputImage> inputImages,
      string targetFolder)
    {
      string contentHash = WebGreaseContext.ComputeContentHash(string.Join("|", inputImages.Select<InputImage, string>((Func<InputImage, string>) (i => i.AbsoluteImagePath))));
      return Path.GetFullPath(Path.Combine(targetFolder, contentHash));
    }

    private static bool IsPhoto(Bitmap bitmap) => bitmap.RawFormat.Equals((object) ImageFormat.Jpeg) || bitmap.RawFormat.Equals((object) ImageFormat.Exif);

    private static bool IsIndexed(Bitmap bitmap) => (bitmap.PixelFormat & PixelFormat.Indexed) != PixelFormat.Undefined;

    private static bool HasAlpha(Bitmap bitmap) => (bitmap.PixelFormat & PixelFormat.Alpha) != PixelFormat.Undefined || (bitmap.PixelFormat & PixelFormat.PAlpha) != PixelFormat.Undefined;

    private static bool IsIndexable(Bitmap bitmap)
    {
      bool flag = false;
      BitArray bitArray = new BitArray(16777216);
      int num = 0;
      int width = bitmap.Width;
      int height = bitmap.Height;
      if (!ImageAssembleGenerator.HasAlpha(bitmap) && width * height <= 256)
        return true;
      for (int x = 0; x < width; ++x)
      {
        for (int y = 0; y < height; ++y)
        {
          Color pixel = bitmap.GetPixel(x, y);
          if (pixel.A == (byte) 0)
          {
            if (!flag)
            {
              ++num;
              flag = true;
            }
          }
          else
          {
            if (pixel.A != byte.MaxValue)
              return false;
            int index = ((int) pixel.R << 16) + ((int) pixel.G << 8) + (int) pixel.B;
            if (!bitArray[index])
            {
              ++num;
              bitArray[index] = true;
            }
          }
          if (num > 256)
            return false;
        }
      }
      return true;
    }

    private static bool IsMultiframe(Bitmap bitmap)
    {
      FrameDimension dimension = new FrameDimension(bitmap.FrameDimensionsList[0]);
      return bitmap.GetFrameCount(dimension) > 1;
    }

    private static ReadOnlyCollection<InputImage> DedupImages(
      ReadOnlyCollection<InputImage> inputImages,
      IWebGreaseContext context)
    {
      List<InputImage> inputImageList = new List<InputImage>();
      Dictionary<string, InputImage> dictionary = new Dictionary<string, InputImage>();
      foreach (InputImage inputImage in inputImages)
      {
        if (!File.Exists(inputImage.AbsoluteImagePath))
          throw new FileNotFoundException("Could not find image to sprite: {0}".InvariantFormat((object) inputImage.AbsoluteImagePath), inputImage.AbsoluteImagePath);
        string key = context.GetFileHash(inputImage.AbsoluteImagePath) + "." + (object) inputImage.Position;
        if (dictionary.ContainsKey(key))
        {
          dictionary[key].DuplicateImagePaths.Add(inputImage.AbsoluteImagePath);
        }
        else
        {
          dictionary.Add(key, inputImage);
          inputImageList.Add(inputImage);
        }
      }
      return inputImageList.AsReadOnly();
    }
  }
}
