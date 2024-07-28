// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts.VersionListsFileUnpacker
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Google.Protobuf;
using Google.Protobuf.Collections;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts
{
  public class VersionListsFileUnpacker
  {
    public static void Unpack(VersionListsFile versionListsFile)
    {
      versionListsFile.ViewIds = versionListsFile.KnownViews.Select<ByteString, Guid>((Func<ByteString, Guid>) (x => new Guid(x.ToByteArray()))).Prepend<Guid>(Guid.Empty).ToList<Guid>();
      PrefixDecompressor prefixDecompressor = new PrefixDecompressor();
      foreach (VersionListsPackage package in versionListsFile.Packages)
      {
        package.Name = new VssNuGetPackageName(prefixDecompressor.DecodeNext(package.CompressedDisplayName));
        package.VersionListsFile = versionListsFile;
      }
    }

    public static void EnsurePackageUnpacked(
      VersionListsFile versionListsFile,
      VersionListsPackage package)
    {
      if (!package.VersionsNeedUnpack)
        return;
      List<Guid> viewIds = versionListsFile.ViewIds;
      PrefixDecompressor prefixDecompressor = new PrefixDecompressor();
      foreach (VersionListsPackageVersion version in package.Versions)
      {
        string versionString = prefixDecompressor.DecodeNext(version.CompressedDisplayVersion);
        version.PackageIdentity = new VssNuGetPackageIdentity(package.Name, new VssNuGetPackageVersion(versionString));
        version.Package = package;
        RepeatedField<int> viewIndices = version.ViewIndices;
        int count = viewIndices.Count;
        List<Guid> guidList = new List<Guid>(count);
        for (int index = 0; index < count; ++index)
          guidList.Add(viewIds[viewIndices[index]]);
        version.ViewIds = guidList;
      }
      package.NotifyVersionsUnpacked();
    }
  }
}
