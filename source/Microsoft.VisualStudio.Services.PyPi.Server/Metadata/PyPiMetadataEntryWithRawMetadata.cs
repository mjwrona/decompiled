// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Metadata.PyPiMetadataEntryWithRawMetadata
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Metadata
{
  public class PyPiMetadataEntryWithRawMetadata : 
    PyPiMetadataEntryBase<PyPiPackageFileWithRawMetadata>,
    IPyPiMetadataEntryWithRawMetadataWritable,
    IPyPiMetadataEntryWithRawMetadata,
    IPyPiMetadataEntryCore<PyPiPackageFileWithRawMetadata>,
    IMetadataEntry<PyPiPackageIdentity>,
    IMetadataEntry,
    IPackageFiles,
    IPackageFiles<PyPiPackageFileWithRawMetadata>,
    ICreateWriteable<IPyPiMetadataEntryWithRawMetadataWritable>,
    IMetadataEntryFilesUpdater<IPyPiMetadataEntryWithRawMetadata>,
    IPyPiMetadataEntryWritableCore<PyPiPackageFileWithRawMetadata>,
    IMetadataEntryWriteable<PyPiPackageIdentity>,
    IMetadataEntryWritable
  {
    public PyPiMetadataEntryWithRawMetadata(PyPiPackageIdentity packageIdentity)
      : base(packageIdentity)
    {
    }

    public PyPiMetadataEntryWithRawMetadata(
      PyPiPackageIdentity packageIdentity,
      PyPiPackageFileWithRawMetadata packageFile,
      VersionConstraintList requiresPython,
      PackagingCommitId commitId,
      Guid createdBy,
      DateTime createdDate,
      Guid modifiedBy,
      DateTime modifiedDate,
      IEnumerable<UpstreamSourceInfo> sourceChain)
      : base(packageIdentity, packageFile, requiresPython, commitId, createdBy, createdDate, modifiedBy, modifiedDate, sourceChain)
    {
    }

    public IPyPiMetadataEntryWithRawMetadataWritable CreateWriteable(
      ICommitLogEntryHeader commitEntry)
    {
      PyPiMetadataEntryWithRawMetadata writeable = new PyPiMetadataEntryWithRawMetadata(this.PackageIdentity);
      writeable.CopyFrom((PyPiMetadataEntryBase<PyPiPackageFileWithRawMetadata>) this);
      writeable.SetUpdatingCommitInfo(commitEntry);
      return (IPyPiMetadataEntryWithRawMetadataWritable) writeable;
    }

    public IPyPiMetadataEntryWithRawMetadata CreateEntryWithUpdatedFiles(
      List<IPackageFile> newPackageFiles)
    {
      PyPiMetadataEntryWithRawMetadata withUpdatedFiles = new PyPiMetadataEntryWithRawMetadata(this.PackageIdentity);
      withUpdatedFiles.CopyFrom((PyPiMetadataEntryBase<PyPiPackageFileWithRawMetadata>) this);
      withUpdatedFiles.PackageFilesInternal = (IReadOnlyCollection<PyPiPackageFileWithRawMetadata>) newPackageFiles.Cast<PyPiPackageFileWithRawMetadata>().ToImmutableList<PyPiPackageFileWithRawMetadata>();
      return (IPyPiMetadataEntryWithRawMetadata) withUpdatedFiles;
    }

    bool IPyPiMetadataEntryWritableCore<PyPiPackageFileWithRawMetadata>.AddPackageFile(
      PyPiPackageFileWithRawMetadata packageFile)
    {
      return this.AddPackageFile(packageFile);
    }

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
