// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.VersionListWithSize.VersionListWithSizeFilePacker
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.VersionListWithSize
{
  public class VersionListWithSizeFilePacker
  {
    public static void Pack(VersionListWithSizeFile versionListWithSizeFile)
    {
      List<VersionListWithSizePackage> list1 = versionListWithSizeFile.Packages.OrderBy<VersionListWithSizePackage, string>((Func<VersionListWithSizePackage, string>) (x => x.DisplayName), (IComparer<string>) StringComparer.Ordinal).ToList<VersionListWithSizePackage>();
      versionListWithSizeFile.Packages.Clear();
      versionListWithSizeFile.Packages.AddRange((IEnumerable<VersionListWithSizePackage>) list1);
      PrefixCompressor prefixCompressor1 = new PrefixCompressor();
      foreach (VersionListWithSizePackage package in versionListWithSizeFile.Packages)
      {
        package.CompressedDisplayName = prefixCompressor1.EncodeNext(package.DisplayName);
        if (package.VersionsNeedRepack)
        {
          List<VersionListWithSizePackageVersion> list2 = package.Versions.OrderBy<VersionListWithSizePackageVersion, string>((Func<VersionListWithSizePackageVersion, string>) (x => x.DisplayVersion), (IComparer<string>) StringComparer.Ordinal).ToList<VersionListWithSizePackageVersion>();
          package.Versions.Clear();
          package.Versions.AddRange((IEnumerable<VersionListWithSizePackageVersion>) list2);
          PrefixCompressor prefixCompressor2 = new PrefixCompressor();
          foreach (VersionListWithSizePackageVersion version in package.Versions)
          {
            version.CompressedDisplayVersion = prefixCompressor2.EncodeNext(version.DisplayVersion);
            List<VersionListWithSizePackageVersionFile> list3 = version.PackageFiles.OrderBy<VersionListWithSizePackageVersionFile, string>((Func<VersionListWithSizePackageVersionFile, string>) (x => x.FileName), (IComparer<string>) StringComparer.Ordinal).ToList<VersionListWithSizePackageVersionFile>();
            version.PackageFiles.Clear();
            version.PackageFiles.AddRange((IEnumerable<VersionListWithSizePackageVersionFile>) list3);
            PrefixCompressor prefixCompressor3 = new PrefixCompressor();
            foreach (VersionListWithSizePackageVersionFile packageFile in version.PackageFiles)
              packageFile.CompressedFileName = prefixCompressor3.EncodeNext(packageFile.FileName);
          }
          package.NotifyVersionsRepacked();
        }
      }
    }
  }
}
