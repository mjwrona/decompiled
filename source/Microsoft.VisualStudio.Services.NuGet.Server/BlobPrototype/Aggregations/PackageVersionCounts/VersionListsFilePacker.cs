// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts.VersionListsFilePacker
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Google.Protobuf;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts
{
  public class VersionListsFilePacker
  {
    public static void Pack(VersionListsFile versionListsFile)
    {
      if (!versionListsFile.KnownViews.Select<ByteString, Guid>((Func<ByteString, Guid>) (x => new Guid(x.ToByteArray()))).Prepend<Guid>(Guid.Empty).ToList<Guid>().SequenceEqual<Guid>((IEnumerable<Guid>) versionListsFile.ViewIds))
        throw new InvalidOperationException("View state out of sync");
      Dictionary<Guid, int> viewIndices = versionListsFile.ViewIds.Select<Guid, (Guid, int)>((Func<Guid, int, (Guid, int)>) ((x, idx) => (x, idx))).ToDictionary<(Guid, int), Guid, int>((Func<(Guid, int), Guid>) (x => x.Item), (Func<(Guid, int), int>) (x => x.Index));
      List<VersionListsPackage> list1 = versionListsFile.Packages.OrderBy<VersionListsPackage, string>((Func<VersionListsPackage, string>) (x => x.Name.DisplayName), (IComparer<string>) StringComparer.Ordinal).ToList<VersionListsPackage>();
      versionListsFile.Packages.Clear();
      versionListsFile.Packages.AddRange((IEnumerable<VersionListsPackage>) list1);
      PrefixCompressor prefixCompressor1 = new PrefixCompressor();
      foreach (VersionListsPackage package in versionListsFile.Packages)
      {
        package.CompressedDisplayName = prefixCompressor1.EncodeNext(package.Name.DisplayName);
        if (package.VersionsNeedRepack)
        {
          List<VersionListsPackageVersion> list2 = package.Versions.OrderBy<VersionListsPackageVersion, string>((Func<VersionListsPackageVersion, string>) (x => x.Version.DisplayVersion), (IComparer<string>) StringComparer.Ordinal).ToList<VersionListsPackageVersion>();
          package.Versions.Clear();
          package.Versions.AddRange((IEnumerable<VersionListsPackageVersion>) list2);
          PrefixCompressor prefixCompressor2 = new PrefixCompressor();
          foreach (VersionListsPackageVersion version in package.Versions)
          {
            version.CompressedDisplayVersion = prefixCompressor2.EncodeNext(version.Version.DisplayVersion);
            IOrderedEnumerable<int> values = version.ViewIds.Select<Guid, int>(new Func<Guid, int>(GetViewIndex)).Where<int>((Func<int, bool>) (x => x != 0)).OrderBy<int, int>((Func<int, int>) (x => x));
            version.ViewIndices.Clear();
            version.ViewIndices.AddRange((IEnumerable<int>) values);
          }
          package.NotifyVersionsRepacked();
        }
      }

      int GetViewIndex(Guid guid)
      {
        int viewIndex;
        if (viewIndices.TryGetValue(guid, out viewIndex))
          return viewIndex;
        int count = viewIndices.Count;
        viewIndices.Add(guid, count);
        versionListsFile.KnownViews.Add(ByteString.CopyFrom(guid.ToByteArray()));
        versionListsFile.ViewIds.Add(guid);
        return count;
      }
    }
  }
}
