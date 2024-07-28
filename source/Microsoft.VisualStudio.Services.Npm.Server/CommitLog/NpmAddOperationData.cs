// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.CommitLog.NpmAddOperationData
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Feed.WebApi.Internal;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog.AdditionalObjects;
using Microsoft.VisualStudio.Services.Npm.Server.PackageIndex;
using Microsoft.VisualStudio.Services.Npm.Server.Utils;
using Microsoft.VisualStudio.Services.Npm.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Npm.Server.CommitLog
{
  public class NpmAddOperationData : 
    INpmAddOperationData,
    IAddOperationData,
    IPackageVersionOperationData,
    IPackageOperationData,
    ICommitOperationData
  {
    private readonly byte[] packageJsonBytes;
    private readonly Lazy<PackageJson> packageJsonLazy;
    private readonly Lazy<SemanticVersion> parsedVersionLazy;

    public NpmAddOperationData(
      IStorageId packageStorageId,
      NpmPackageName packageName,
      long packageSize,
      byte[] packageJsonBytes,
      string packageSha1Sum,
      string distTag,
      bool isUpstreamCached,
      PackageJsonOptions packageJsonOptions,
      PackageManifest packageManifest,
      IEnumerable<UpstreamSourceInfo> sourceChain,
      ProvenanceInfo provenance,
      IEnumerable<Guid> packageViews,
      string deprecateMessage)
    {
      NpmAddOperationData addOperationData = this;
      this.packageJsonBytes = (byte[]) packageJsonBytes.Clone();
      this.PackageSha1Sum = packageSha1Sum;
      this.packageJsonLazy = new Lazy<PackageJson>((Func<PackageJson>) (() =>
      {
        DeflateCompressibleBytes packageJsonBytes1 = DeflateCompressibleBytes.FromUncompressedBytes(packageJsonBytes);
        string fullName = packageName?.FullName;
        IEnumerable<UpstreamSourceInfo> source = sourceChain;
        string location = source != null ? source.FirstOrDefault<UpstreamSourceInfo>()?.Location : (string) null;
        IReadOnlyList<Exception> exceptionList;
        ref IReadOnlyList<Exception> local = ref exceptionList;
        return PackageJsonUtils.DeserializeNpmJsonDocument<PackageJson>(packageJsonBytes1, (ITracerService) null, fullName, (string) null, location, nameof (NpmAddOperationData), out local);
      }));
      this.parsedVersionLazy = new Lazy<SemanticVersion>((Func<SemanticVersion>) (() => NpmVersionUtils.ParseNpmPackageVersion(addOperationData.packageJsonLazy.Value.Version)));
      this.PackageSize = packageSize;
      this.PackageStorageId = packageStorageId;
      this.DistTag = distTag;
      this.IsUpstreamCached = isUpstreamCached;
      this.PackageJsonOptions = packageJsonOptions ?? new PackageJsonOptions();
      PackageManifest packageManifest1 = packageManifest;
      if (packageManifest1 == null)
        packageManifest1 = new PackageManifest()
        {
          FilesMetadata = new Dictionary<string, PackageFileMetadata>()
        };
      this.PackageManifest = packageManifest1;
      this.SourceChain = sourceChain ?? Enumerable.Empty<UpstreamSourceInfo>();
      this.Provenance = provenance;
      this.DeprecateMessage = deprecateMessage;
      this.PackageViews = packageViews ?? Enumerable.Empty<Guid>();
      this.PackageName = (IPackageName) (packageName ?? new NpmPackageName(this.packageJsonLazy.Value.Name));
      this.Identity = (IPackageIdentity<NpmPackageName, SemanticVersion>) new NpmPackageIdentity(this.PackageJson.Name, this.parsedVersionLazy.Value);
    }

    public IPackageName PackageName { get; }

    public IStorageId PackageStorageId { get; }

    public byte[] PackageJsonBytes => (byte[]) this.packageJsonBytes.Clone();

    public long PackageSize { get; }

    public string PackageSha1Sum { get; }

    public string DistTag { get; }

    public bool IsUpstreamCached { get; }

    public PackageJsonOptions PackageJsonOptions { get; }

    public PackageManifest PackageManifest { get; }

    public PackageJson PackageJson => this.packageJsonLazy.Value;

    IPackageIdentity IPackageVersionOperationData.Identity => (IPackageIdentity) this.Identity;

    public RingOrder RingOrder => RingOrder.InnerToOuter;

    public FeedPermissionConstants PermissionDemand => PackageIngestionUtils.GetRequiredAddPackagePermission(this.GetIngestionDirection());

    public IPackageIdentity<NpmPackageName, SemanticVersion> Identity { get; }

    public IEnumerable<UpstreamSourceInfo> SourceChain { get; }

    public ProvenanceInfo Provenance { get; }

    public string DeprecateMessage { get; }

    public IEnumerable<Guid> PackageViews { get; }

    public PackageIndexEntry ConvertToIndexEntry(ICommitLogEntry commitLogEntry, FeedCore feed) => this.ConvertToIndexEntry(commitLogEntry, feed.Capabilities.HasFlag((Enum) FeedCapabilities.UpstreamV2));

    public PackageIndexEntry ConvertToIndexEntry(
      ICommitLogEntry commitLogEntry,
      bool ignoreCachedFlag)
    {
      List<PackageDependency> packageDependencyList = new List<PackageDependency>();
      packageDependencyList.AddRange(NpmAddOperationData.GetDependencies(this.PackageJson.Dependencies, (string) null));
      packageDependencyList.AddRange(NpmAddOperationData.GetDependencies(this.PackageJson.PeerDependencies, "peerDependencies"));
      packageDependencyList.AddRange(NpmAddOperationData.GetDependencies(this.PackageJson.DevDependencies, "devDependencies"));
      packageDependencyList.AddRange(NpmAddOperationData.GetDependencies(this.PackageJson.OptionalDependencies, "optionalDependencies"));
      if (this.PackageJson.BundleDependencies != null)
        packageDependencyList.AddRange(ConvertBundledDependencies(this.PackageJson.BundleDependencies));
      if (this.PackageJson.BundledDependencies != null)
        packageDependencyList.AddRange(ConvertBundledDependencies(this.PackageJson.BundledDependencies));
      return new PackageIndexEntry()
      {
        Name = this.Identity.Name.FullName,
        NormalizedName = this.Identity.Name.FullName,
        ProtocolType = "Npm",
        PackageVersion = new PackageVersionIndexEntry()
        {
          Description = this.PackageJson.Description,
          NormalizedVersion = this.Identity.Version.NormalizedVersion,
          SortableVersion = NpmAddOperationData.GetSortableVersion(this.Identity.Version.NormalizedVersion),
          Version = this.Identity.Version.DisplayVersion,
          StorageId = this.PackageStorageId.ValueString,
          PublishDate = new DateTime?(commitLogEntry.CreatedDate),
          Tags = (IEnumerable<string>) this.PackageJson.Keywords,
          Dependencies = (IEnumerable<PackageDependency>) packageDependencyList,
          Author = this.PackageJson.Author != null ? this.PackageJson.Author.FullName() : (string) null,
          Summary = (string) null,
          VersionProtocolMetadata = PackageJsonUtils.GetProtocolMetadata(this.PackageJson),
          IsRelease = !this.parsedVersionLazy.Value.IsPrerelease,
          IsCached = this.IsUpstreamCached && !ignoreCachedFlag,
          SourceChain = this.SourceChain.Select<UpstreamSourceInfo, UpstreamSource>((Func<UpstreamSourceInfo, UpstreamSource>) (x => x.ToFeedApiSource("Npm"))),
          Provenance = ProvenanceUtils.GetFeedProvenance(this.Provenance, commitLogEntry.UserId)
        }
      };

      IEnumerable<PackageDependency> ConvertBundledDependencies(
        BundledDependencies bundledDependencies)
      {
        return (!bundledDependencies.BundleAllDependencies ? (IEnumerable<string>) bundledDependencies.List : ((IEnumerable<string>) this.PackageJson.Dependencies?.Keys ?? Enumerable.Empty<string>()).Concat<string>((IEnumerable<string>) this.PackageJson.OptionalDependencies?.Keys ?? Enumerable.Empty<string>())).Select<string, PackageDependency>((Func<string, PackageDependency>) (x => new PackageDependency()
        {
          PackageName = x,
          VersionRange = string.Empty,
          Group = nameof (bundledDependencies)
        }));
      }
    }

    private static IEnumerable<PackageDependency> GetDependencies(
      Dictionary<string, string> dependencies,
      string group)
    {
      if (dependencies != null)
      {
        foreach (KeyValuePair<string, string> dependency in dependencies)
          yield return new PackageDependency()
          {
            PackageName = dependency.Key,
            VersionRange = dependency.Value,
            Group = group
          };
      }
    }

    private static string GetSortableVersion(string normalizedVersion) => new NpmSortableVersionBuilder().GetSortableVersion(normalizedVersion);
  }
}
