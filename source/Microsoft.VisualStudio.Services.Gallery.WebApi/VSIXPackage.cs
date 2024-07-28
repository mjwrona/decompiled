// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.WebApi.VSIXPackage
// Assembly: Microsoft.VisualStudio.Services.Gallery.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EE9D0AAA-B110-4AD6-813B-50FA04AC401A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Gallery.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Gallery.WebApi
{
  public class VSIXPackage
  {
    public static HashSet<string> PersistedAssetTypes = new HashSet<string>((IEnumerable<string>) new List<string>()
    {
      "Microsoft.VisualStudio.Services.VSIXPackage",
      "Microsoft.VisualStudio.Services.Manifest",
      "Microsoft.VisualStudio.Services.Icons.Default",
      "Microsoft.VisualStudio.Services.Icons.Wide",
      "Microsoft.VisualStudio.Services.Icons.Small",
      "Microsoft.VisualStudio.Services.VsixManifest"
    }, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private static XmlReaderSettings s_readerSettings = new XmlReaderSettings()
    {
      ConformanceLevel = ConformanceLevel.Fragment,
      DtdProcessing = DtdProcessing.Prohibit,
      XmlResolver = (XmlResolver) null
    };
    private static string ExtensionPartName = "/extension.package";
    private static string VSIXPartName = "/extension.vsixmanifest";
    private static string SmallIconPartName = "/Microsoft.VisualStudio.Services.Icons.Small";
    private const long DefaultMaxPackageSize = 209715200;
    private static Uri ExtensionPackagePart = new Uri(VSIXPackage.ExtensionPartName, UriKind.Relative);
    private static Uri VSIXExtensionPart = new Uri(VSIXPackage.VSIXPartName, UriKind.Relative);
    private static readonly int c_MaxDegreeOfParallelism = 16;

    private static void ValidatePackageDetailsManifest(
      PackageDetails packageDetails,
      bool skipVersionCheck)
    {
      if (packageDetails.Manifest == null || packageDetails.Manifest.Metadata == null || packageDetails.Manifest.Metadata.Identity == null)
        throw new InvalidPackageFormatException(GalleryWebApiResources.MissingPackageManifest());
      if (!skipVersionCheck && (packageDetails.Manifest.Metadata.Identity.Version == null || packageDetails.Manifest.Metadata.Identity.Version.Length > 43 || !Version.TryParse(packageDetails.Manifest.Metadata.Identity.Version, out Version _)))
        throw new InvalidPackageFormatException(GalleryWebApiResources.InvalidExtensionVersion((object) packageDetails.Manifest.Metadata.Identity.Version));
    }

    public static PackageDetails Parse(Stream packageStream) => VSIXPackage.Parse(packageStream, (Func<ManifestFile, Stream, bool>) null);

    public static PackageDetails Parse(
      Stream packageStream,
      Func<ManifestFile, Stream, bool> assetAvailable)
    {
      return VSIXPackage.Parse(packageStream, (IImageResizeUtility) new DefaultImageResizeUtility((Func<Exception, bool>) null, false), assetAvailable);
    }

    public static PackageDetails Parse(
      Stream packageStream,
      IImageResizeUtility imageResizeUtility,
      Func<ManifestFile, Stream, bool> assetAvailable)
    {
      return VSIXPackage.Parse(packageStream, imageResizeUtility, false, assetAvailable);
    }

    public static PackageDetails Parse(
      Stream packageStream,
      IImageResizeUtility imageResizeUtility,
      bool isMultithreaded,
      Func<ManifestFile, Stream, bool> assetAvailable)
    {
      PackageDetails packageDetails = new PackageDetails();
      packageStream.Seek(0L, SeekOrigin.Begin);
      using (Package package = Package.Open(packageStream))
      {
        if (package == null)
          throw new InvalidPackageFormatException(GalleryWebApiResources.InvalidPackageStream());
        PackagePart packagePart = (PackagePart) null;
        foreach (PackagePart part in package.GetParts())
        {
          if (part.GetStream().Length > 209715200L)
            throw new ExtensionSizeExceededException(packageStream.Length, 209715200L);
          if (part.Uri == VSIXPackage.ExtensionPackagePart)
            throw new InvalidPackageFormatException(GalleryWebApiResources.InvalidPackageFormat((object) GalleryWebApiResources.UseOfResistrictedPart((object) part.Uri.ToString())));
          if (part.Uri.ToString().EndsWith(".marketplacemanifest") || part.Uri.ToString().EndsWith(".vsixmanifest"))
          {
            packagePart = part;
            using (StreamReader input = new StreamReader(part.GetStream(), Encoding.UTF8))
            {
              using (XmlReader xmlReader = XmlReader.Create((TextReader) input, VSIXPackage.s_readerSettings))
              {
                try
                {
                  PackageManifestInternal packageManifestInternal = (PackageManifestInternal) new XmlSerializer(typeof (PackageManifestInternal)).Deserialize(xmlReader);
                  packageDetails.Manifest = new PackageManifest(packageManifestInternal);
                }
                catch (Exception ex)
                {
                  if (ex.InnerException != null && ex.InnerException is InvalidOperationException)
                    throw new InvalidPackageFormatException(GalleryWebApiResources.InvalidPackageManifest((object) ex.InnerException.Message));
                  throw;
                }
                if (packageDetails.Manifest.Metadata != null)
                {
                  if (!string.IsNullOrEmpty(packageDetails.Manifest.Metadata.Categories))
                    packageDetails.Categories = ((IEnumerable<string>) packageDetails.Manifest.Metadata.Categories.Split(',')).Select<string, string>((Func<string, string>) (category => category != null ? category.Trim() : category));
                  if (!string.IsNullOrEmpty(packageDetails.Manifest.Metadata.Tags))
                  {
                    packageDetails.Tags = ((IEnumerable<string>) packageDetails.Manifest.Metadata.Tags.Split(',')).Select<string, string>((Func<string, string>) (tag => tag != null ? tag.Trim() : tag));
                    break;
                  }
                  break;
                }
                break;
              }
            }
          }
        }
        VSIXPackage.ValidatePackageDetailsManifest(packageDetails, GalleryUtil.IsVsVsixTypeZipPackage(packageDetails));
        if (assetAvailable != null)
        {
          ManifestFile sourceIconManifestFile = (ManifestFile) null;
          Stream sourceIconStream = (Stream) null;
          bool flag = false;
          Dictionary<string, Tuple<ManifestFile, Stream>> dictionary = new Dictionary<string, Tuple<ManifestFile, Stream>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          ConcurrentBag<ManifestFile> assetToInclude = new ConcurrentBag<ManifestFile>();
          try
          {
            foreach (ManifestFile asset in packageDetails.Manifest.Assets)
            {
              string key = string.Format("{0}::{1}", (object) asset.AssetType, (object) asset.Language);
              if (!dictionary.ContainsKey(key))
              {
                asset.FullPath = asset.FullPath.StartsWith("/") ? asset.FullPath : "/" + asset.FullPath;
                asset.Version = packageDetails.Manifest.Metadata.Identity.Version;
                PackagePart part = package.GetPart(new Uri(asset.FullPath, UriKind.Relative));
                if (part != null)
                {
                  asset.ContentType = part.ContentType;
                  Stream stream1 = part.GetStream();
                  if (string.Equals(asset.AssetType, "Microsoft.VisualStudio.Services.Icons.Small", StringComparison.OrdinalIgnoreCase) && imageResizeUtility != null && imageResizeUtility.ShouldCreateSmallImages)
                  {
                    flag = true;
                    Stream stream2 = imageResizeUtility.ResizeImage(stream1, 72, 153, part.ContentType);
                    if (stream2 != null)
                    {
                      stream1 = stream2;
                      asset.ContentType = imageResizeUtility.OutputContentType;
                    }
                  }
                  stream1.Seek(0L, SeekOrigin.Begin);
                  MemoryStream destination = new MemoryStream();
                  stream1.CopyTo((Stream) destination);
                  destination.Seek(0L, SeekOrigin.Begin);
                  dictionary.Add(key, new Tuple<ManifestFile, Stream>(asset, (Stream) destination));
                }
                if (string.Equals(asset.AssetType, "Microsoft.VisualStudio.Services.Icons.Default", StringComparison.OrdinalIgnoreCase))
                {
                  sourceIconManifestFile = asset;
                  sourceIconStream = part.GetStream();
                }
              }
            }
            if (isMultithreaded)
            {
              Dictionary<string, Tuple<ManifestFile, Stream>>.ValueCollection values = dictionary.Values;
              ParallelOptions parallelOptions = new ParallelOptions();
              parallelOptions.MaxDegreeOfParallelism = VSIXPackage.c_MaxDegreeOfParallelism;
              Action<Tuple<ManifestFile, Stream>> body = (Action<Tuple<ManifestFile, Stream>>) (assetStream =>
              {
                if (!assetAvailable(assetStream.Item1, assetStream.Item2))
                  return;
                assetToInclude.Add(assetStream.Item1);
              });
              Parallel.ForEach<Tuple<ManifestFile, Stream>>((IEnumerable<Tuple<ManifestFile, Stream>>) values, parallelOptions, body);
            }
            else
              dictionary.Values.ForEach<Tuple<ManifestFile, Stream>>((Action<Tuple<ManifestFile, Stream>>) (assetStream =>
              {
                if (!assetAvailable(assetStream.Item1, assetStream.Item2))
                  return;
                assetToInclude.Add(assetStream.Item1);
              }));
          }
          finally
          {
            if (isMultithreaded)
              Parallel.ForEach<Tuple<ManifestFile, Stream>>((IEnumerable<Tuple<ManifestFile, Stream>>) dictionary.Values, (Action<Tuple<ManifestFile, Stream>>) (assetStream =>
              {
                if (assetStream.Item2 == null)
                  return;
                assetStream.Item2.Dispose();
              }));
            else
              dictionary.Values.ForEach<Tuple<ManifestFile, Stream>>((Action<Tuple<ManifestFile, Stream>>) (assetStream =>
              {
                if (assetStream.Item2 == null)
                  return;
                assetStream.Item2.Dispose();
              }));
          }
          packageDetails.Manifest.Assets.Clear();
          assetToInclude.ForEach<ManifestFile>((Action<ManifestFile>) (asset => packageDetails.Manifest.Assets.Add(asset)));
          if (!flag && sourceIconManifestFile != null)
            VSIXPackage.AddSmallIconAsset(imageResizeUtility, sourceIconStream, sourceIconManifestFile, assetAvailable, packageDetails);
          if (packagePart != null)
            VSIXPackage.AddVSIXManifestAsset(packagePart, assetAvailable, packageDetails);
        }
      }
      if (assetAvailable != null)
      {
        ManifestFile manifestFile1 = new ManifestFile();
        manifestFile1.FullPath = VSIXPackage.ExtensionPartName;
        manifestFile1.ContentType = "application/zip";
        manifestFile1.AssetType = "Microsoft.VisualStudio.Services.VSIXPackage";
        manifestFile1.Version = packageDetails.Manifest.Metadata.Identity.Version;
        ManifestFile manifestFile2 = manifestFile1;
        packageStream.Seek(0L, SeekOrigin.Begin);
        if (assetAvailable(manifestFile2, packageStream))
          packageDetails.Manifest.Assets.Add(manifestFile2);
      }
      return packageDetails;
    }

    private static void AddSmallIconAsset(
      IImageResizeUtility imageResizeUtility,
      Stream sourceIconStream,
      ManifestFile sourceIconManifestFile,
      Func<ManifestFile, Stream, bool> assetAvailable,
      PackageDetails packageDetails)
    {
      if (imageResizeUtility == null || !imageResizeUtility.ShouldCreateSmallImages)
        return;
      Stream stream = imageResizeUtility.ResizeImage(sourceIconStream, 72, 153, sourceIconManifestFile.ContentType);
      ManifestFile manifestFile1 = new ManifestFile();
      manifestFile1.FullPath = VSIXPackage.SmallIconPartName;
      manifestFile1.ContentType = stream == null ? sourceIconManifestFile.ContentType : imageResizeUtility.OutputContentType;
      manifestFile1.AssetType = "Microsoft.VisualStudio.Services.Icons.Small";
      manifestFile1.Version = sourceIconManifestFile.Version;
      ManifestFile manifestFile2 = manifestFile1;
      if (stream != null)
      {
        if (assetAvailable(manifestFile2, stream))
          packageDetails.Manifest.Assets.Add(manifestFile2);
        stream.Close();
      }
      else
      {
        sourceIconStream.Seek(0L, SeekOrigin.Begin);
        manifestFile2.FullPath = sourceIconManifestFile.FullPath;
        if (!assetAvailable(manifestFile2, sourceIconStream))
          return;
        manifestFile2.FullPath = VSIXPackage.SmallIconPartName;
        packageDetails.Manifest.Assets.Add(manifestFile2);
      }
    }

    private static void AddVSIXManifestAsset(
      PackagePart packagePart,
      Func<ManifestFile, Stream, bool> assetAvailable,
      PackageDetails packageDetails)
    {
      Stream stream = packagePart.GetStream(FileMode.Open, FileAccess.Read);
      ManifestFile manifestFile1 = new ManifestFile();
      manifestFile1.Addressable = true;
      manifestFile1.FullPath = VSIXPackage.VSIXPartName;
      manifestFile1.ContentType = packagePart.ContentType;
      manifestFile1.AssetType = "Microsoft.VisualStudio.Services.VsixManifest";
      manifestFile1.Version = packageDetails.Manifest.Metadata.Identity.Version;
      ManifestFile manifestFile2 = manifestFile1;
      if (stream == null || !assetAvailable(manifestFile2, stream))
        return;
      packageDetails.Manifest.Assets.Add(manifestFile2);
    }

    public static void ExtractParts(
      Stream packageStream,
      Action<Uri, string, Stream> partAvailable,
      FileAccess packageStreamAccess = FileAccess.Read)
    {
      using (Package package = Package.Open(packageStream, FileMode.Open, packageStreamAccess))
      {
        foreach (PackagePart part in package.GetParts())
          partAvailable(part.Uri, part.ContentType, part.GetStream(FileMode.Open, packageStreamAccess));
      }
    }

    public static string ExtractPublisherNameFromPackageJson(Stream packageStream)
    {
      using (Package package = Package.Open(packageStream))
      {
        if (package == null)
          throw new InvalidPackageFormatException(GalleryWebApiResources.InvalidPackageStream());
        foreach (PackagePart part in package.GetParts())
        {
          if (part.Uri.ToString().Equals("/extension/package.json", StringComparison.OrdinalIgnoreCase))
          {
            try
            {
              using (StreamReader reader1 = new StreamReader(part.GetStream(), Encoding.UTF8))
              {
                using (JsonTextReader reader2 = new JsonTextReader((TextReader) reader1))
                {
                  JObject jobject = JsonSerializer.CreateDefault().Deserialize<JObject>((JsonReader) reader2);
                  if (jobject != null)
                    return jobject["publisher"].Value<string>();
                }
              }
            }
            catch (Exception ex)
            {
              throw new InvalidPackageFormatException(GalleryWebApiResources.InvalidPackageFormat((object) ex.Message));
            }
          }
        }
        return (string) null;
      }
    }

    public static void ExtractManifest(
      Stream packageStream,
      Action<Uri, string, Stream> partAvailable,
      FileAccess packageStreamAccess = FileAccess.Read)
    {
      using (Package package = Package.Open(packageStream, FileMode.Open, packageStreamAccess))
      {
        foreach (PackagePart part in package.GetParts())
        {
          if (part.Uri.ToString().EndsWith(".vsixmanifest"))
          {
            using (Stream stream = part.GetStream(FileMode.Open, packageStreamAccess))
              partAvailable(part.Uri, part.ContentType, stream);
          }
        }
      }
    }

    public static void ExtractFileByFilename(
      Stream packageStream,
      string fileName,
      Action<Stream> fileAvailable)
    {
      if (packageStream == null || string.IsNullOrEmpty(fileName) || fileAvailable == null)
        return;
      packageStream.Seek(0L, SeekOrigin.Begin);
      using (Package package = Package.Open(packageStream))
      {
        if (package == null)
          throw new InvalidPackageFormatException(GalleryWebApiResources.InvalidPackageStream());
        fileName = fileName.StartsWith("/") ? fileName : "/" + fileName;
        PackagePart part = package.GetPart(new Uri(fileName, UriKind.Relative));
        if (part == null)
          return;
        fileAvailable(part.GetStream());
      }
    }
  }
}
