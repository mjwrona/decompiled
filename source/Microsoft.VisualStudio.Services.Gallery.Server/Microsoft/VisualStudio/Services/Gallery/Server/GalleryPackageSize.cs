// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.GalleryPackageSize
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public static class GalleryPackageSize
  {
    public const long OneMB = 1048576;
    public const long DefaultMaxPackageSize = 104857600;
    public const long VsCodeDefaultMaxPackageSize = 209715200;
    public const long VsDefaultMaxPackageSize = 482344960;
    public const long DraftAssetMaxPackageSize = 2097152;
    public const int DraftAssetTimeoutMinutes = 5;
    public const int VstsVsCodePackageTimeoutMinutes = 10;
    public const int VsPackageTimeoutMinutes = 60;
    public const long PublisherAssetMaxSize = 2097152;
    public const int PublisherAssetTimeoutMinutes = 10;
    public const string LargeExtensionUploadRoot = "/Configuration/Service/Gallery/LargeExtensionUpload/";
    public const string MaxPackageSizeRegistryPath = "/Configuration/Service/Gallery/LargeExtensionUpload/MaxPackageSizeMB";
    public const string PackageTimeoutMinutesRegistryPath = "/Configuration/Service/Gallery/LargeExtensionUpload/PackageTimeoutMins";
    public const string GalleryPackageFileName = "Gallery_PackageFileName";
    public const string GalleryPackageSignatureArchiveFileName = "Gallery_SignatureArchiveFileName";
    public const string MaxPackageSizeMB = "MaxPackageSizeMB";
  }
}
