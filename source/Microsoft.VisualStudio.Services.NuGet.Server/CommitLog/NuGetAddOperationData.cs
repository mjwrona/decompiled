// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.CommitLog.NuGetAddOperationData
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Feed.WebApi.Internal;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageIndex;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Microsoft.VisualStudio.Services.NuGet.Server.CommitLog
{
  public class NuGetAddOperationData : 
    INuGetAddOperationData,
    IAddOperationData,
    IPackageVersionOperationData,
    IPackageOperationData,
    ICommitOperationData,
    IAddOperationDataSupportsAddAsDelisted
  {
    private readonly byte[] nuspecBytes;
    private readonly Lazy<NuGetPackageMetadata> metadataLazy;

    public NuGetAddOperationData(
      VssNuGetPackageIdentity packageIdentity,
      IStorageId packageStorageId,
      long packageSize,
      byte[] nuspecBytes,
      IEnumerable<UpstreamSourceInfo> sourceChain,
      ProvenanceInfo provenance,
      bool addAsDelisted)
    {
      this.PackageStorageId = packageStorageId;
      this.PackageSize = packageSize;
      this.nuspecBytes = (byte[]) nuspecBytes.Clone();
      this.metadataLazy = new Lazy<NuGetPackageMetadata>((Func<NuGetPackageMetadata>) (() => new NuGetPackageMetadata(this.NuspecBytes, false, MetadataReadOptions.IgnoreNonCriticalErrors)));
      this.Identity = packageIdentity ?? this.Metadata.Identity;
      this.SourceChain = sourceChain ?? Enumerable.Empty<UpstreamSourceInfo>();
      this.Provenance = provenance;
      this.AddAsDelisted = addAsDelisted;
    }

    public byte[] NuspecBytes => (byte[]) this.nuspecBytes.Clone();

    IPackageIdentity IPackageVersionOperationData.Identity => (IPackageIdentity) this.Identity;

    public VssNuGetPackageIdentity Identity { get; }

    public IStorageId PackageStorageId { get; }

    public long PackageSize { get; }

    public IPackageName PackageName => (IPackageName) this.Identity.Name;

    public RingOrder RingOrder => RingOrder.InnerToOuter;

    public FeedPermissionConstants PermissionDemand => PackageIngestionUtils.GetRequiredAddPackagePermission(this.GetIngestionDirection());

    public NuGetPackageMetadata Metadata => this.metadataLazy.Value;

    public IEnumerable<UpstreamSourceInfo> SourceChain { get; }

    public ProvenanceInfo Provenance { get; }

    public bool AddAsDelisted { get; }

    public PackageIndexEntry ConvertToIndexEntry(ICommitLogEntry commitLogEntry, FeedCore feed)
    {
      NuGetPackageMetadata metadata = this.Metadata;
      return new PackageIndexEntry()
      {
        Name = this.Identity.Name.DisplayName,
        NormalizedName = this.Identity.Name.NormalizedName,
        ProtocolType = "NuGet",
        PackageVersion = new PackageVersionIndexEntry()
        {
          Author = metadata.Authors,
          Description = metadata.Description,
          Summary = metadata.Summary,
          NormalizedVersion = this.Identity.Version.NormalizedVersion,
          SortableVersion = this.GetSortableVersion(this.Identity.Version.NormalizedVersion),
          Version = this.Identity.Version.DisplayVersion,
          Tags = (IEnumerable<string>) (metadata.Tags ?? ImmutableList<string>.Empty),
          StorageId = this.PackageStorageId.ValueString,
          PublishDate = new DateTime?(commitLogEntry.CreatedDate),
          Dependencies = this.ConvertToPackageDependencyCollection((IEnumerable<NuGetDependencyGroup>) metadata.DependencyGroups),
          VersionProtocolMetadata = NuGetMetadataFactory.Create(metadata),
          IsRelease = !metadata.Identity.Version.NuGetVersion.IsPrerelease,
          IsCached = false,
          SourceChain = this.SourceChain.Select<UpstreamSourceInfo, UpstreamSource>((Func<UpstreamSourceInfo, UpstreamSource>) (x => x.ToFeedApiSource("NuGet"))),
          Provenance = ProvenanceUtils.GetFeedProvenance(this.Provenance, commitLogEntry.UserId)
        }
      };
    }

    private string GetSortableVersion(string normalizedVersion) => new NuGetSortableVersionBuilder().GetSortableVersion(normalizedVersion);

    private IEnumerable<PackageDependency> ConvertToPackageDependencyCollection(
      IEnumerable<NuGetDependencyGroup> dependencyGroups)
    {
      List<PackageDependency> dependencyCollection = new List<PackageDependency>();
      foreach (NuGetDependencyGroup dependencyGroup1 in dependencyGroups)
      {
        NuGetDependencyGroup dependencyGroup = dependencyGroup1;
        dependencyCollection.AddRange(dependencyGroup.Dependencies.Select<NuGetDependency, PackageDependency>((Func<NuGetDependency, PackageDependency>) (x => new PackageDependency()
        {
          PackageName = x.Name.DisplayName,
          Group = dependencyGroup.TargetFramework,
          VersionRange = x.Range.ToString()
        })));
      }
      return (IEnumerable<PackageDependency>) dependencyCollection;
    }
  }
}
