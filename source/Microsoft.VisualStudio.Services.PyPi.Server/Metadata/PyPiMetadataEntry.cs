// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Metadata.PyPiMetadataEntry
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
  public class PyPiMetadataEntry : 
    PyPiMetadataEntryBase<PyPiPackageFile>,
    IPyPiMetadataEntryWritable,
    IPyPiMetadataEntry,
    IPyPiMetadataEntryCore<PyPiPackageFile>,
    IMetadataEntry<PyPiPackageIdentity>,
    IMetadataEntry,
    IPackageFiles,
    IPackageFiles<PyPiPackageFile>,
    ICreateWriteable<IPyPiMetadataEntryWritable>,
    IMetadataEntryFilesUpdater<IPyPiMetadataEntry>,
    IPyPiMetadataEntryWritableCore<PyPiPackageFile>,
    IMetadataEntryWriteable<PyPiPackageIdentity>,
    IMetadataEntryWritable
  {
    public PyPiMetadataEntry(PyPiPackageIdentity packageIdentity)
      : base(packageIdentity)
    {
    }

    public PyPiMetadataEntry(
      PyPiPackageIdentity packageIdentity,
      PyPiPackageFile packageFile,
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

    public IPyPiMetadataEntryWritable CreateWriteable(ICommitLogEntryHeader commitEntry)
    {
      PyPiMetadataEntry writeable = new PyPiMetadataEntry(this.PackageIdentity);
      writeable.CopyFrom((PyPiMetadataEntryBase<PyPiPackageFile>) this);
      writeable.SetUpdatingCommitInfo(commitEntry);
      return (IPyPiMetadataEntryWritable) writeable;
    }

    public IPyPiMetadataEntry CreateEntryWithUpdatedFiles(List<IPackageFile> newPackageFiles)
    {
      PyPiMetadataEntry withUpdatedFiles = new PyPiMetadataEntry(this.PackageIdentity);
      withUpdatedFiles.CopyFrom((PyPiMetadataEntryBase<PyPiPackageFile>) this);
      withUpdatedFiles.PackageFilesInternal = (IReadOnlyCollection<PyPiPackageFile>) newPackageFiles.Cast<PyPiPackageFile>().ToImmutableList<PyPiPackageFile>();
      return (IPyPiMetadataEntry) withUpdatedFiles;
    }

    bool IPyPiMetadataEntryWritableCore<PyPiPackageFile>.AddPackageFile(PyPiPackageFile packageFile) => this.AddPackageFile(packageFile);

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
