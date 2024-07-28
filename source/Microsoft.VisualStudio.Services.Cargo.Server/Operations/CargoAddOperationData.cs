// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Operations.CargoAddOperationData
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Cargo.Server.Ingestion;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity.VersionDetails;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Feed.WebApi.Internal;
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


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Operations
{
  public class CargoAddOperationData : 
    IAddOperationData,
    IPackageVersionOperationData,
    IPackageOperationData,
    ICommitOperationData,
    IAddOperationDataSupportsAddAsDelisted
  {
    public CargoPackageIdentity Identity { get; }

    public IStorageId PackageStorageId { get; }

    public long PackageSize { get; }

    public LazySerDesValue<CargoRawPackageMetadata, CargoPackageMetadata> Metadata { get; }

    public ImmutableArray<InnerFileReference> InnerFiles { get; }

    public IReadOnlyList<HashAndType> Hashes { get; }

    public IEnumerable<UpstreamSourceInfo> SourceChain { get; }

    public ProvenanceInfo? Provenance { get; }

    public bool AddAsYanked { get; }

    public CargoAddOperationData(
      CargoPackageIdentity identity,
      IStorageId packageStorageId,
      long packageSize,
      LazySerDesValue<CargoRawPackageMetadata, CargoPackageMetadata> metadata,
      IEnumerable<HashAndType> hashes,
      IEnumerable<UpstreamSourceInfo> sourceChain,
      ProvenanceInfo? provenance,
      ImmutableArray<InnerFileReference> innerFiles,
      bool addAsYanked)
    {
      this.Identity = identity;
      this.PackageStorageId = packageStorageId;
      this.PackageSize = packageSize;
      this.Metadata = metadata;
      this.InnerFiles = innerFiles;
      this.AddAsYanked = addAsYanked;
      this.Hashes = (IReadOnlyList<HashAndType>) hashes.ToImmutableList<HashAndType>();
      this.SourceChain = (IEnumerable<UpstreamSourceInfo>) sourceChain.ToImmutableList<UpstreamSourceInfo>();
      this.Provenance = provenance;
    }

    public PackageIndexEntry ConvertToIndexEntry(ICommitLogEntry commitLogEntry, FeedCore feed)
    {
      CargoPackageMetadata cargoPackageMetadata = this.Metadata.Value;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return new PackageIndexEntry()
      {
        Name = this.Identity.Name.DisplayName,
        NormalizedName = this.Identity.Name.NormalizedName,
        ProtocolType = Protocol.Cargo.CorrectlyCasedName,
        PackageVersion = new PackageVersionIndexEntry()
        {
          Author = string.Join(", ", (IEnumerable<string>) cargoPackageMetadata.Authors),
          Description = cargoPackageMetadata.Description,
          Summary = (string) null,
          NormalizedVersion = this.Identity.Version.NormalizedVersion,
          SortableVersion = CargoSortableVersionBuilder.Instance.GenerateSortableVersion(this.Identity.Version),
          Version = this.Identity.Version.DisplayVersion,
          Tags = (IEnumerable<string>) cargoPackageMetadata.Keywords,
          StorageId = this.PackageStorageId.ValueString,
          PublishDate = new DateTime?(commitLogEntry.CreatedDate),
          Dependencies = cargoPackageMetadata.Dependencies.Select<CargoPackageMetadataDependency, PackageDependency>(CargoAddOperationData.\u003C\u003EO.\u003C0\u003E__TransformDependency ?? (CargoAddOperationData.\u003C\u003EO.\u003C0\u003E__TransformDependency = new Func<CargoPackageMetadataDependency, PackageDependency>(TransformDependency))),
          VersionProtocolMetadata = new ProtocolMetadata()
          {
            SchemaVersion = 1,
            Data = (object) new CargoFeedIndexMetadata(cargoPackageMetadata.Features, (IImmutableList<string>) cargoPackageMetadata.Authors, cargoPackageMetadata.DocumentationUrl, cargoPackageMetadata.HomepageUrl, cargoPackageMetadata.ReadmeText, cargoPackageMetadata.ReadmeFilePath, (IImmutableList<string>) cargoPackageMetadata.Categories, cargoPackageMetadata.LicenseExpression, cargoPackageMetadata.LicenseFilePath, cargoPackageMetadata.RepositoryUrl, cargoPackageMetadata.Links)
          },
          IsRelease = this.Identity.Version.PrereleaseLabel == null,
          IsCached = false,
          SourceChain = this.SourceChain.Select<UpstreamSourceInfo, UpstreamSource>((Func<UpstreamSourceInfo, UpstreamSource>) (x => x.ToFeedApiSource(Protocol.Cargo.CorrectlyCasedName))),
          Provenance = ProvenanceUtils.GetFeedProvenance(this.Provenance, commitLogEntry.UserId)
        }
      };

      static PackageDependency TransformDependency(CargoPackageMetadataDependency arg) => new PackageDependency()
      {
        Group = arg.Kind,
        PackageName = string.IsNullOrWhiteSpace(arg.DeclaredName) ? arg.ActualPackageName.DisplayName : arg.DeclaredName,
        VersionRange = arg.VersionRequirement
      };
    }

    public RingOrder RingOrder => RingOrder.InnerToOuter;

    public FeedPermissionConstants PermissionDemand => PackageIngestionUtils.GetRequiredAddPackagePermission(this.GetIngestionDirection());

    IPackageName IPackageOperationData.PackageName => (IPackageName) this.Identity.Name;

    IPackageIdentity IPackageVersionOperationData.Identity => (IPackageIdentity) this.Identity;

    bool IAddOperationDataSupportsAddAsDelisted.AddAsDelisted => this.AddAsYanked;
  }
}
