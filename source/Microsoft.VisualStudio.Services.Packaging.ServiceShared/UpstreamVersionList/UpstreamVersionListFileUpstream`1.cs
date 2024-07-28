// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList.UpstreamVersionListFileUpstream`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList
{
  public class UpstreamVersionListFileUpstream<TPackageVersion> : 
    IUpstreamVersionListFileUpstream<TPackageVersion>
    where TPackageVersion : IPackageVersion
  {
    public UpstreamSourceInfo UpstreamSourceInfo { get; }

    public IImmutableList<VersionWithSourceChain<TPackageVersion>> Versions { get; }

    IReadOnlyList<IVersionWithSourceChain<TPackageVersion>> IUpstreamVersionListFileUpstream<TPackageVersion>.Versions => (IReadOnlyList<IVersionWithSourceChain<TPackageVersion>>) this.Versions;

    public DateTime LastRefreshed { get; }

    public UpstreamVersionListFileUpstream(
      UpstreamSourceInfo upstreamSourceInfo,
      IEnumerable<VersionWithSourceChain<TPackageVersion>> versions,
      DateTime lastRefreshed)
    {
      this.UpstreamSourceInfo = upstreamSourceInfo;
      this.LastRefreshed = lastRefreshed;
      this.Versions = (IImmutableList<VersionWithSourceChain<TPackageVersion>>) versions.ToImmutableList<VersionWithSourceChain<TPackageVersion>>();
    }

    private UpstreamVersionListFileUpstream<TPackageVersion>.StoredVersion PackVersion(
      VersionWithSourceChain<TPackageVersion> versionWithSourceChain,
      SourceChainMap sourceChainMap)
    {
      int sourceChainIndex = sourceChainMap.GetOrAddSourceChainIndex(versionWithSourceChain.SourceChain);
      return new UpstreamVersionListFileUpstream<TPackageVersion>.StoredVersion()
      {
        Version = versionWithSourceChain.Version.DisplayVersion,
        SourceChainIndex = sourceChainIndex
      };
    }

    internal UpstreamVersionListFileUpstream<TPackageVersion>.StoredUpstream Pack(
      SourceChainMap sourceChainMap)
    {
      ImmutableList<UpstreamVersionListFileUpstream<TPackageVersion>.StoredVersion> immutableList = this.Versions.Select<VersionWithSourceChain<TPackageVersion>, UpstreamVersionListFileUpstream<TPackageVersion>.StoredVersion>((Func<VersionWithSourceChain<TPackageVersion>, UpstreamVersionListFileUpstream<TPackageVersion>.StoredVersion>) (x => this.PackVersion(x, sourceChainMap))).ToImmutableList<UpstreamVersionListFileUpstream<TPackageVersion>.StoredVersion>();
      return new UpstreamVersionListFileUpstream<TPackageVersion>.StoredUpstream()
      {
        UpstreamSourceInfo = this.UpstreamSourceInfo,
        Versions = (IEnumerable<UpstreamVersionListFileUpstream<TPackageVersion>.StoredVersion>) immutableList,
        LastRefreshed = this.LastRefreshed
      };
    }

    internal class StoredUpstream
    {
      public UpstreamSourceInfo UpstreamSourceInfo { get; set; }

      public IEnumerable<UpstreamVersionListFileUpstream<TPackageVersion>.StoredVersion> Versions { get; set; }

      public DateTime LastRefreshed { get; set; }

      public UpstreamVersionListFileUpstream<TPackageVersion> Unpack(
        Func<string, TPackageVersion> versionConvertFunc,
        ImmutableList<ImmutableList<UpstreamSourceInfo>> sourceChainMap)
      {
        return new UpstreamVersionListFileUpstream<TPackageVersion>(this.UpstreamSourceInfo, this.Versions.Select<UpstreamVersionListFileUpstream<TPackageVersion>.StoredVersion, VersionWithSourceChain<TPackageVersion>>((Func<UpstreamVersionListFileUpstream<TPackageVersion>.StoredVersion, VersionWithSourceChain<TPackageVersion>>) (x => x.Unpack(versionConvertFunc, sourceChainMap))), this.LastRefreshed);
      }
    }

    internal class StoredVersion
    {
      public string Version { get; set; }

      public int SourceChainIndex { get; set; }

      public VersionWithSourceChain<TPackageVersion> Unpack(
        Func<string, TPackageVersion> versionConvertFunc,
        ImmutableList<ImmutableList<UpstreamSourceInfo>> sourceChainMap)
      {
        return VersionWithSourceChain<TPackageVersion>.FromThisFeed(versionConvertFunc(this.Version), (IEnumerable<UpstreamSourceInfo>) sourceChainMap[this.SourceChainIndex]);
      }
    }
  }
}
