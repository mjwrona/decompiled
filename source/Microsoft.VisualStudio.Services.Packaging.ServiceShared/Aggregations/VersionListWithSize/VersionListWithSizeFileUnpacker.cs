// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.VersionListWithSize.VersionListWithSizeFileUnpacker
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.VersionListWithSize
{
  internal class VersionListWithSizeFileUnpacker
  {
    public static void Unpack(VersionListWithSizeFile versionListWithSizeFile)
    {
      PrefixDecompressor prefixDecompressor = new PrefixDecompressor();
      foreach (VersionListWithSizePackage package in versionListWithSizeFile.Packages)
      {
        package.DisplayName = prefixDecompressor.DecodeNext(package.CompressedDisplayName);
        package.VersionListWithSizeFile = versionListWithSizeFile;
      }
    }

    public static void EnsurePackageUnpacked(VersionListWithSizePackage package)
    {
      if (!package.VersionsNeedUnpack)
        return;
      PrefixDecompressor prefixDecompressor1 = new PrefixDecompressor();
      foreach (VersionListWithSizePackageVersion version in package.Versions)
      {
        string str1 = prefixDecompressor1.DecodeNext(version.CompressedDisplayVersion);
        version.DisplayVersion = str1;
        version.Package = package;
        PrefixDecompressor prefixDecompressor2 = new PrefixDecompressor();
        foreach (VersionListWithSizePackageVersionFile packageFile in version.PackageFiles)
        {
          string str2 = prefixDecompressor2.DecodeNext(packageFile.CompressedFileName);
          packageFile.FileName = str2;
          packageFile.Version = version;
        }
      }
      package.NotifyVersionsUnpacked();
    }
  }
}
