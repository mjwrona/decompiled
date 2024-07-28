// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList.UpstreamVersionListFile`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList
{
  public class UpstreamVersionListFile<TPackageVersion> : IUpstreamVersionListFile<TPackageVersion> where TPackageVersion : IPackageVersion
  {
    public IImmutableList<UpstreamVersionListFileUpstream<TPackageVersion>> Upstreams { get; }

    IReadOnlyList<IUpstreamVersionListFileUpstream<TPackageVersion>> IUpstreamVersionListFile<TPackageVersion>.Upstreams => (IReadOnlyList<IUpstreamVersionListFileUpstream<TPackageVersion>>) this.Upstreams;

    public static UpstreamVersionListFile<TPackageVersion> Empty { get; } = new UpstreamVersionListFile<TPackageVersion>((IEnumerable<UpstreamVersionListFileUpstream<TPackageVersion>>) ImmutableList<UpstreamVersionListFileUpstream<TPackageVersion>>.Empty);

    public UpstreamVersionListFile(
      IEnumerable<UpstreamVersionListFileUpstream<TPackageVersion>> upstreams)
    {
      this.Upstreams = (IImmutableList<UpstreamVersionListFileUpstream<TPackageVersion>>) upstreams.ToImmutableList<UpstreamVersionListFileUpstream<TPackageVersion>>();
    }

    public UpstreamVersionListFile<TPackageVersion> WithoutDataOlderThan(DateTime minimumRefreshTime) => new UpstreamVersionListFile<TPackageVersion>((IEnumerable<UpstreamVersionListFileUpstream<TPackageVersion>>) this.Upstreams.RemoveAll((Predicate<UpstreamVersionListFileUpstream<TPackageVersion>>) (x => x.LastRefreshed < minimumRefreshTime)));

    IUpstreamVersionListFile<TPackageVersion> IUpstreamVersionListFile<TPackageVersion>.WithoutDataOlderThan(
      DateTime minimumRefreshTime)
    {
      return (IUpstreamVersionListFile<TPackageVersion>) this.WithoutDataOlderThan(minimumRefreshTime);
    }

    internal UpstreamVersionListFile<TPackageVersion>.Stored Pack()
    {
      SourceChainMap sourceChainMap = new SourceChainMap();
      ImmutableList<UpstreamVersionListFileUpstream<TPackageVersion>.StoredUpstream> immutableList = this.Upstreams.Select<UpstreamVersionListFileUpstream<TPackageVersion>, UpstreamVersionListFileUpstream<TPackageVersion>.StoredUpstream>((Func<UpstreamVersionListFileUpstream<TPackageVersion>, UpstreamVersionListFileUpstream<TPackageVersion>.StoredUpstream>) (x => x.Pack(sourceChainMap))).ToImmutableList<UpstreamVersionListFileUpstream<TPackageVersion>.StoredUpstream>();
      return new UpstreamVersionListFile<TPackageVersion>.Stored()
      {
        Upstreams = (IEnumerable<UpstreamVersionListFileUpstream<TPackageVersion>.StoredUpstream>) immutableList,
        SourceChainMap = (IEnumerable<IEnumerable<UpstreamSourceInfo>>) sourceChainMap.SourceChains
      };
    }

    public UpstreamVersionListFile<TPackageVersion> WithAddedOrUpdatedUpstream(
      UpstreamVersionListFileUpstream<TPackageVersion> upstreamInfo)
    {
      return new UpstreamVersionListFile<TPackageVersion>((IEnumerable<UpstreamVersionListFileUpstream<TPackageVersion>>) this.Upstreams.RemoveAll((Predicate<UpstreamVersionListFileUpstream<TPackageVersion>>) (x => x.UpstreamSourceInfo.Id == upstreamInfo.UpstreamSourceInfo.Id)).Add(upstreamInfo));
    }

    public UpstreamVersionListFile<TPackageVersion> WithoutUpstreamsByUpstreamId(
      IEnumerable<Guid> upstreamsToRemove)
    {
      ISet<Guid> upstreamsToRemoveSet = upstreamsToRemove as ISet<Guid>;
      if (upstreamsToRemoveSet == null)
        upstreamsToRemoveSet = (ISet<Guid>) new HashSet<Guid>(upstreamsToRemove);
      return new UpstreamVersionListFile<TPackageVersion>((IEnumerable<UpstreamVersionListFileUpstream<TPackageVersion>>) this.Upstreams.RemoveAll((Predicate<UpstreamVersionListFileUpstream<TPackageVersion>>) (x => upstreamsToRemoveSet.Contains(x.UpstreamSourceInfo.Id))));
    }

    IUpstreamVersionListFile<TPackageVersion> IUpstreamVersionListFile<TPackageVersion>.WithoutUpstreamsByUpstreamId(
      IEnumerable<Guid> upstreamsToRemove)
    {
      return (IUpstreamVersionListFile<TPackageVersion>) this.WithoutUpstreamsByUpstreamId(upstreamsToRemove);
    }

    internal class Stored
    {
      public IEnumerable<IEnumerable<UpstreamSourceInfo>> SourceChainMap { get; set; }

      public IEnumerable<UpstreamVersionListFileUpstream<TPackageVersion>.StoredUpstream> Upstreams { get; set; }

      internal UpstreamVersionListFile<TPackageVersion> Unpack(
        Func<string, TPackageVersion> versionConvertFunc)
      {
        ImmutableList<ImmutableList<UpstreamSourceInfo>> sourceChainMapPrepped = this.SourceChainMap.Select<IEnumerable<UpstreamSourceInfo>, ImmutableList<UpstreamSourceInfo>>((Func<IEnumerable<UpstreamSourceInfo>, ImmutableList<UpstreamSourceInfo>>) (x => x.ToImmutableList<UpstreamSourceInfo>())).ToImmutableList<ImmutableList<UpstreamSourceInfo>>();
        return new UpstreamVersionListFile<TPackageVersion>(this.Upstreams.Select<UpstreamVersionListFileUpstream<TPackageVersion>.StoredUpstream, UpstreamVersionListFileUpstream<TPackageVersion>>((Func<UpstreamVersionListFileUpstream<TPackageVersion>.StoredUpstream, UpstreamVersionListFileUpstream<TPackageVersion>>) (x => x.Unpack(versionConvertFunc, sourceChainMapPrepped))));
      }
    }
  }
}
