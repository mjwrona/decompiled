// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata.CargoMetadataEntry
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Cargo.Server.Ingestion;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata
{
  public class CargoMetadataEntry : 
    MetadataEntryWithSingleFile<CargoPackageIdentity>,
    ICargoMetadataEntryWriteable,
    IMetadataEntryWriteable<CargoPackageIdentity>,
    IMetadataEntry<CargoPackageIdentity>,
    IMetadataEntry,
    IPackageFiles,
    IMetadataEntryWritable,
    ICargoMetadataEntry,
    ICreateWriteable<ICargoMetadataEntryWriteable>
  {
    private readonly CargoPackageIdentity? packageId;

    public LazySerDesValue<CargoRawPackageMetadata, CargoPackageMetadata>? Metadata { get; set; }

    public IReadOnlyList<HashAndType> Hashes { get; set; } = (IReadOnlyList<HashAndType>) ImmutableList<HashAndType>.Empty;

    public bool Yanked { get; set; }

    public ImmutableArray<InnerFileReference> CrateInnerFiles { get; set; } = ImmutableArray<InnerFileReference>.Empty;

    public CargoMetadataEntry(
      PackagingCommitId commitId,
      DateTime createdDate,
      DateTime modifiedDate,
      Guid createdBy,
      Guid modifiedBy,
      IStorageId packageStorageId,
      long packageSize,
      LazySerDesValue<CargoRawPackageMetadata, CargoPackageMetadata> metadata,
      IEnumerable<UpstreamSourceInfo> sourceChain,
      IEnumerable<HashAndType> hashes,
      IEnumerable<InnerFileReference> crateInnerFiles,
      bool yanked)
      : base(commitId, createdDate, modifiedDate, createdBy, modifiedBy, packageStorageId, packageSize, sourceChain)
    {
      this.Hashes = (IReadOnlyList<HashAndType>) hashes.ToImmutableArray<HashAndType>();
      this.Metadata = metadata;
      this.Yanked = yanked;
      this.CrateInnerFiles = crateInnerFiles.ToImmutableArray<InnerFileReference>();
    }

    public CargoMetadataEntry(CargoPackageIdentity packageIdentity) => this.packageId = packageIdentity;

    public ICargoMetadataEntryWriteable CreateWriteable(ICommitLogEntryHeader commitEntry)
    {
      CargoMetadataEntry writeable = new CargoMetadataEntry(this.PackageIdentity);
      writeable.CopyFrom(this);
      writeable.SetUpdatingCommitInfo(commitEntry);
      return (ICargoMetadataEntryWriteable) writeable;
    }

    public override CargoPackageIdentity PackageIdentity
    {
      get
      {
        CargoPackageIdentity packageId = this.packageId;
        if (packageId != null)
          return packageId;
        return this.Metadata?.Value.Identity ?? throw new InvalidOperationException("Either the identity or the metadata must be set before this object is exposed.");
      }
    }

    public override string SingleFilePath => this.PackageIdentity.GetCanonicalCrateFileName();

    protected override IReadOnlyCollection<HashAndType> SingleFileHashes => (IReadOnlyCollection<HashAndType>) this.Hashes;

    protected override IReadOnlyCollection<InnerFileReference> SingleFileInnerFiles => (IReadOnlyCollection<InnerFileReference>) this.CrateInnerFiles;

    public override IEqualityComparer<string> PathEqualityComparer => (IEqualityComparer<string>) StringComparer.Ordinal;

    protected void CopyFrom(CargoMetadataEntry metadataEntry)
    {
      this.CopyFrom((MetadataEntry<CargoPackageIdentity, IPackageFile>) metadataEntry);
      this.Metadata = metadataEntry.Metadata;
      this.Hashes = metadataEntry.Hashes;
      this.Yanked = metadataEntry.Yanked;
      this.CrateInnerFiles = metadataEntry.CrateInnerFiles;
    }

    void IMetadataEntryWritable.set_CommitId(PackagingCommitId value) => this.CommitId = value;

    void IMetadataEntryWritable.set_CreatedBy(Guid value) => this.CreatedBy = value;

    void IMetadataEntryWritable.set_CreatedDate(DateTime value) => this.CreatedDate = value;

    void IMetadataEntryWritable.set_ModifiedBy(Guid value) => this.ModifiedBy = value;

    void IMetadataEntryWritable.set_ModifiedDate(DateTime value) => this.ModifiedDate = value;

    void IMetadataEntryWritable.set_PackageStorageId(
    #nullable disable
    IStorageId value) => this.PackageStorageId = value;

    void IMetadataEntryWritable.set_PackageSize(long value) => this.PackageSize = value;

    void IMetadataEntryWritable.set_Views(IEnumerable<Guid> value) => this.Views = value;

    void IMetadataEntryWritable.set_DeletedDate(DateTime? value) => this.DeletedDate = value;

    void IMetadataEntryWritable.set_ScheduledPermanentDeleteDate(DateTime? value) => this.ScheduledPermanentDeleteDate = value;

    void IMetadataEntryWritable.set_PermanentDeletedDate(DateTime? value) => this.PermanentDeletedDate = value;

    void IMetadataEntryWritable.set_QuarantineUntilDate(DateTime? value) => this.QuarantineUntilDate = value;

    void IMetadataEntryWritable.set_SourceChain(IEnumerable<UpstreamSourceInfo> value) => this.SourceChain = value;
  }
}
