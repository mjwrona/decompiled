// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NuGetMetadataEntry
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class NuGetMetadataEntry : 
    MetadataEntryWithSingleFile<VssNuGetPackageIdentity>,
    INuGetMetadataEntryWriteable,
    INuGetMetadataEntry,
    IMetadataEntry<VssNuGetPackageIdentity>,
    IMetadataEntry,
    IPackageFiles,
    ICreateWriteable<INuGetMetadataEntryWriteable>,
    IMetadataEntryWriteable<VssNuGetPackageIdentity>,
    IMetadataEntryWritable
  {
    private readonly VssNuGetPackageIdentity packageId;
    private readonly Lazy<NuGetPackageMetadata> metadataLazy;
    private readonly Lazy<IReadOnlyCollection<InnerFileReference>> innerFilesLazy;

    public NuGetMetadataEntry(
      PackagingCommitId commitId,
      DateTime createdDate,
      DateTime modifiedDate,
      Guid createdBy,
      Guid modifiedBy,
      IStorageId packageStorageId,
      long packageSize,
      ContentBytes nuspecBytes,
      bool listed,
      IEnumerable<UpstreamSourceInfo> sourceChain)
      : base(commitId, createdDate, modifiedDate, createdBy, modifiedBy, packageStorageId, packageSize, sourceChain)
    {
      this.Listed = listed;
      if (nuspecBytes != null)
        this.NuspecBytes = (byte[]) nuspecBytes.Content.Clone();
      this.AreBytesCompressed = nuspecBytes != null && nuspecBytes.AreBytesCompressed;
      this.metadataLazy = new Lazy<NuGetPackageMetadata>(new Func<NuGetPackageMetadata>(this.InitializeMetadata));
      this.innerFilesLazy = new Lazy<IReadOnlyCollection<InnerFileReference>>(new Func<IReadOnlyCollection<InnerFileReference>>(this.InitializeInnerFiles));
    }

    public NuGetMetadataEntry(VssNuGetPackageIdentity packageId)
    {
      this.packageId = packageId;
      this.metadataLazy = new Lazy<NuGetPackageMetadata>(new Func<NuGetPackageMetadata>(this.InitializeMetadata));
      this.innerFilesLazy = new Lazy<IReadOnlyCollection<InnerFileReference>>(new Func<IReadOnlyCollection<InnerFileReference>>(this.InitializeInnerFiles));
    }

    private NuGetPackageMetadata InitializeMetadata() => this.NuspecBytes == null ? (NuGetPackageMetadata) null : new NuGetPackageMetadata(this.NuspecBytes, this.AreBytesCompressed, MetadataReadOptions.IgnoreNonCriticalErrors, this.Listed);

    protected override IReadOnlyCollection<InnerFileReference> SingleFileInnerFiles => this.innerFilesLazy.Value;

    private IReadOnlyCollection<InnerFileReference> InitializeInnerFiles()
    {
      if (this.Metadata == null || this.PackageStorageId == null)
        return (IReadOnlyCollection<InnerFileReference>) ImmutableArray<InnerFileReference>.Empty;
      ImmutableArray<InnerFileReference>.Builder builder = ImmutableArray.CreateBuilder<InnerFileReference>();
      AddIfPresent(this.Metadata.IconFile);
      AddIfPresent(this.Metadata.LicenseFile);
      return (IReadOnlyCollection<InnerFileReference>) builder.ToImmutable();

      void AddIfPresent(string path)
      {
        if (string.IsNullOrWhiteSpace(path))
          return;
        builder.Add(new InnerFileReference(path, (IStorageId) new ExtractFileFromZipStorageId(this.PackageStorageId, path), -1L));
      }
    }

    public bool AreBytesCompressed { get; private set; }

    public byte[] NuspecBytes { get; private set; }

    public NuGetPackageMetadata Metadata => this.metadataLazy.Value;

    public override VssNuGetPackageIdentity PackageIdentity => this.packageId != null ? this.packageId : this.Metadata.Identity;

    public void SetNuspecBytes(byte[] bytes, bool areBytesCompressed)
    {
      this.NuspecBytes = bytes;
      this.AreBytesCompressed = areBytesCompressed;
    }

    public bool Listed { get; set; }

    public INuGetMetadataEntryWriteable CreateWriteable(ICommitLogEntryHeader commitEntry)
    {
      NuGetMetadataEntry writeable = new NuGetMetadataEntry(this.PackageIdentity);
      writeable.CopyFrom((MetadataEntry<VssNuGetPackageIdentity, IPackageFile>) this);
      writeable.SetUpdatingCommitInfo(commitEntry);
      writeable.NuspecBytes = (byte[]) this.NuspecBytes?.Clone();
      writeable.AreBytesCompressed = this.AreBytesCompressed;
      writeable.Listed = this.Listed;
      return (INuGetMetadataEntryWriteable) writeable;
    }

    public override IEqualityComparer<string> PathEqualityComparer { get; } = (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase;

    public override string SingleFilePath => this.PackageIdentity.ToNupkgFilePath();

    protected override string FixupFilePath(string filePath) => !filePath.EndsWith(".nupkg", StringComparison.OrdinalIgnoreCase) ? filePath : this.SingleFilePath;

    void IMetadataEntryWritable.set_CommitId(PackagingCommitId value) => this.CommitId = value;

    void IMetadataEntryWritable.set_CreatedBy(Guid value) => this.CreatedBy = value;

    void IMetadataEntryWritable.set_CreatedDate(DateTime value) => this.CreatedDate = value;

    void IMetadataEntryWritable.set_ModifiedBy(Guid value) => this.ModifiedBy = value;

    void IMetadataEntryWritable.set_ModifiedDate(DateTime value) => this.ModifiedDate = value;

    void IMetadataEntryWritable.set_PackageStorageId(IStorageId value) => this.PackageStorageId = value;

    void IMetadataEntryWritable.set_PackageSize(long value) => this.PackageSize = value;

    void IMetadataEntryWritable.set_Views(IEnumerable<Guid> value) => this.Views = value;

    void IMetadataEntryWritable.set_DeletedDate(DateTime? value) => this.DeletedDate = value;

    void IMetadataEntryWritable.set_ScheduledPermanentDeleteDate(DateTime? value) => this.ScheduledPermanentDeleteDate = value;

    void IMetadataEntryWritable.set_PermanentDeletedDate(DateTime? value) => this.PermanentDeletedDate = value;

    void IMetadataEntryWritable.set_QuarantineUntilDate(DateTime? value) => this.QuarantineUntilDate = value;

    void IMetadataEntryWritable.set_SourceChain(IEnumerable<UpstreamSourceInfo> value) => this.SourceChain = value;
  }
}
