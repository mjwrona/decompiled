// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.MetadataEntry`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public abstract class MetadataEntry<TPackageIdentity, TPackageFile> : 
    IMetadataEntry<TPackageIdentity>,
    IMetadataEntry,
    IPackageFiles,
    IPackageFiles<TPackageFile>
    where TPackageIdentity : IPackageIdentity
    where TPackageFile : IPackageFile
  {
    protected MetadataEntry()
    {
    }

    protected MetadataEntry(
      PackagingCommitId commitId,
      DateTime createdDate,
      DateTime modifiedDate,
      Guid createdBy,
      Guid modifiedBy,
      IStorageId packageStorageId,
      long packageSize,
      IEnumerable<UpstreamSourceInfo> sourceChain)
    {
      this.CommitId = commitId;
      this.CreatedDate = createdDate;
      this.ModifiedDate = modifiedDate;
      this.PackageStorageId = packageStorageId;
      this.PackageSize = packageSize;
      this.CreatedBy = createdBy;
      this.ModifiedBy = modifiedBy;
      this.SourceChain = sourceChain != null ? (IEnumerable<UpstreamSourceInfo>) sourceChain.ToImmutableList<UpstreamSourceInfo>() : (IEnumerable<UpstreamSourceInfo>) null;
    }

    protected void CopyFrom(
      MetadataEntry<TPackageIdentity, TPackageFile> source)
    {
      this.CommitId = source.CommitId;
      this.CreatedBy = source.CreatedBy;
      this.CreatedDate = source.CreatedDate;
      this.DeletedDate = source.DeletedDate;
      this.ModifiedBy = source.ModifiedBy;
      this.ModifiedDate = source.ModifiedDate;
      this.PackageSize = source.PackageSize;
      this.PackageStorageId = source.PackageStorageId;
      this.PermanentDeletedDate = source.PermanentDeletedDate;
      this.ScheduledPermanentDeleteDate = source.ScheduledPermanentDeleteDate;
      this.QuarantineUntilDate = source.QuarantineUntilDate;
      IEnumerable<UpstreamSourceInfo> sourceChain = source.SourceChain;
      this.SourceChain = sourceChain != null ? (IEnumerable<UpstreamSourceInfo>) sourceChain.ToImmutableList<UpstreamSourceInfo>() : (IEnumerable<UpstreamSourceInfo>) null;
      IEnumerable<Guid> views = source.Views;
      this.Views = views != null ? (IEnumerable<Guid>) views.ToImmutableList<Guid>() : (IEnumerable<Guid>) null;
    }

    protected void SetUpdatingCommitInfo(ICommitLogEntryHeader commit)
    {
      this.CommitId = commit.CommitId;
      this.ModifiedBy = commit.UserId;
      this.ModifiedDate = commit.CreatedDate;
    }

    public DateTime ModifiedDate { get; set; }

    public DateTime? PermanentDeletedDate { get; set; }

    public IStorageId PackageStorageId { get; set; }

    public long PackageSize { get; set; }

    public PackagingCommitId CommitId { get; set; }

    public DateTime CreatedDate { get; set; }

    public Guid CreatedBy { get; set; }

    public IEnumerable<Guid> Views { get; set; }

    public DateTime? DeletedDate { get; set; }

    public DateTime? ScheduledPermanentDeleteDate { get; set; }

    public DateTime? QuarantineUntilDate { get; set; }

    public Guid ModifiedBy { get; set; }

    public abstract TPackageIdentity PackageIdentity { get; }

    public virtual ICachablePackageMetadata GetCachableMetadata(IPackageFileRequest request)
    {
      if (request is IPackageInnerFileRequest innerFileRequest && !string.IsNullOrWhiteSpace(innerFileRequest.InnerFilePath))
      {
        InnerFileReference innerFileWithPath = this.GetPackageInnerFileWithPath(innerFileRequest.FilePath, innerFileRequest.InnerFilePath);
        return (object) innerFileWithPath != null ? (ICachablePackageMetadata) new CachablePackageMetadata((IMetadataEntry) this, innerFileWithPath) : (ICachablePackageMetadata) null;
      }
      TPackageFile packageFileWithPath = this.GetPackageFileWithPath(request.FilePath);
      return (object) packageFileWithPath != null ? (ICachablePackageMetadata) new CachablePackageMetadata((IMetadataEntry) this, (IPackageFile) packageFileWithPath) : (ICachablePackageMetadata) null;
    }

    IPackageIdentity IMetadataEntry.PackageIdentity => (IPackageIdentity) this.PackageIdentity;

    public IEnumerable<UpstreamSourceInfo> SourceChain { get; set; }

    public virtual bool IsLocal => this.PackageStorageId != null && this.PackageStorageId.IsLocal || this.PackageFiles.Any<TPackageFile>((Func<TPackageFile, bool>) (f => f.StorageId.IsLocal));

    public virtual bool IsFromUpstream => this.SourceChain != null && this.SourceChain.Any<UpstreamSourceInfo>() || this.PackageStorageId != null && !this.PackageStorageId.IsLocal || this.PackageFiles.Any<TPackageFile>((Func<TPackageFile, bool>) (f => !f.StorageId.IsLocal));

    public bool IsUpstreamCached => false;

    IReadOnlyCollection<IPackageFile> IPackageFiles.PackageFiles => (IReadOnlyCollection<IPackageFile>) this.PackageFiles;

    public abstract IReadOnlyCollection<TPackageFile> PackageFiles { get; }

    IPackageFile IPackageFiles.GetPackageFileWithPath(string filePath) => (IPackageFile) this.GetPackageFileWithPath(filePath);

    public TPackageFile GetPackageFileWithPath(string filePath)
    {
      filePath = this.FixupFilePath(filePath);
      IEqualityComparer<string> pathEqualityComparer = this.PathEqualityComparer;
      return this.PackageFiles.FirstOrDefault<TPackageFile>((Func<TPackageFile, bool>) (f => pathEqualityComparer.Equals(f.Path, filePath)));
    }

    public InnerFileReference GetPackageInnerFileWithPath(string filePath, string innerFilePath)
    {
      TPackageFile packageFileWithPath = this.GetPackageFileWithPath(filePath);
      if ((object) packageFileWithPath == null)
        return (InnerFileReference) null;
      IEqualityComparer<string> pathEqualityComparer = this.PathEqualityComparer;
      return packageFileWithPath.InnerFiles.FirstOrDefault<InnerFileReference>((Func<InnerFileReference, bool>) (f => pathEqualityComparer.Equals(f.Path, innerFilePath)));
    }

    protected virtual string FixupFilePath(string filePath) => filePath;

    public abstract IEqualityComparer<string> PathEqualityComparer { get; }
  }
}
