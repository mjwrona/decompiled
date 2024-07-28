// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpdateUpstreamMetadataOperationData`4
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.CentralFeedServices;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  [System.Diagnostics.DebuggerDisplay("{DebuggerDisplay,nq}")]
  public class UpdateUpstreamMetadataOperationData<TPackageIdentity, TPackageName, TPackageVersion, TMetadataEntry> : 
    IUpdateUpstreamMetadataOperationData<TPackageIdentity, TPackageName, TPackageVersion, TMetadataEntry>,
    IUpdateUpstreamMetadataOperationData<TPackageName, TPackageVersion>,
    IUpdateUpstreamMetadataOperationData,
    IPackageOperationData,
    ICommitOperationData
    where TPackageIdentity : IPackageIdentity<TPackageName, TPackageVersion>
    where TPackageName : IPackageName
    where TPackageVersion : IPackageVersion
    where TMetadataEntry : IMetadataEntry<TPackageIdentity>
  {
    public UpdateUpstreamMetadataOperationData(
      TPackageName packageName,
      string upstreamsConfigurationHash,
      DateTime upstreamsLastRefreshedUtc,
      List<TMetadataEntry> upstreamEntriesToAddOrUpdate,
      object packageNameMetadata,
      IEnumerable<UpstreamVersionListFileUpstream<TPackageVersion>> newUpstreamVersionListsToCache,
      DateTime minimumUpstreamVersionListDate,
      Dictionary<TPackageVersion, TerrapinIngestionValidationReason> entriesToUpdateWithTerrapinData)
    {
      this.PackageName = packageName;
      this.UpstreamsConfigurationHash = upstreamsConfigurationHash;
      this.UpstreamsLastRefreshedUtc = upstreamsLastRefreshedUtc;
      this.UpstreamEntries = upstreamEntriesToAddOrUpdate;
      this.PackageNameMetadata = packageNameMetadata;
      this.NewUpstreamVersionListsToCache = newUpstreamVersionListsToCache.ToImmutableList<UpstreamVersionListFileUpstream<TPackageVersion>>();
      this.MinimumUpstreamVersionListDate = minimumUpstreamVersionListDate;
      this.EntriesToUpdateWithTerrapinData = entriesToUpdateWithTerrapinData;
    }

    private string DebuggerDisplay => string.Format("Package: {0}, Entries: {1}, Hash: {2}...", (object) this.PackageName, (object) this.UpstreamEntries.Count, (object) this.UpstreamsConfigurationHash.Substring(0, 4));

    public TPackageName PackageName { get; }

    public string UpstreamsConfigurationHash { get; }

    public DateTime UpstreamsLastRefreshedUtc { get; }

    public List<TMetadataEntry> UpstreamEntries { get; }

    public object PackageNameMetadata { get; }

    public ImmutableList<UpstreamVersionListFileUpstream<TPackageVersion>> NewUpstreamVersionListsToCache { get; }

    public DateTime MinimumUpstreamVersionListDate { get; }

    public Dictionary<TPackageVersion, TerrapinIngestionValidationReason> EntriesToUpdateWithTerrapinData { get; }

    public RingOrder RingOrder => RingOrder.InnerToOuter;

    public FeedPermissionConstants PermissionDemand => FeedPermissionConstants.ReadPackages;

    IPackageName IPackageOperationData.PackageName => (IPackageName) this.PackageName;
  }
}
