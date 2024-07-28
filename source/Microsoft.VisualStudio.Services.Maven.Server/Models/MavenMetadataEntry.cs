// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Models.MavenMetadataEntry
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Maven.Server.Metadata;
using Microsoft.VisualStudio.Services.Maven.Server.Utilities;
using Microsoft.VisualStudio.Services.Maven.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Maven.Server.Models
{
  public class MavenMetadataEntry : 
    MetadataEntryWithFiles<MavenPackageIdentity, MavenPackageFileNew>,
    IMavenMetadataEntryWritable,
    IMavenMetadataEntry,
    IMetadataEntry<MavenPackageIdentity>,
    IMetadataEntry,
    IPackageFiles,
    IPackageFiles<MavenPackageFileNew>,
    IMetadataEntryFilesUpdater<IMavenMetadataEntry>,
    ICreateWriteable<IMavenMetadataEntryWritable>,
    IMetadataEntryWriteable<MavenPackageIdentity>,
    IMetadataEntryWritable
  {
    public byte[] PomBytes { get; set; }

    public override MavenPackageIdentity PackageIdentity { get; }

    public MavenPomMetadata Pom => this.PomBytes == null ? (MavenPomMetadata) null : MavenPomUtility.Parse((Stream) new MemoryStream(this.PomBytes));

    public MavenMetadataEntry(
      MavenPackageIdentity packageIdentity,
      PackagingCommitId commitId,
      DateTime createdDate,
      DateTime modifiedDate,
      Guid createdBy,
      Guid modifiedBy,
      IEnumerable<UpstreamSourceInfo> sourceChain,
      IEnumerable<MavenPackageFileNew> packageFiles,
      byte[] pomBytes)
      : base(commitId, createdDate, modifiedDate, createdBy, modifiedBy, sourceChain, packageFiles)
    {
      this.PackageIdentity = packageIdentity ?? throw new ArgumentNullException(nameof (packageIdentity));
      this.PomBytes = pomBytes;
    }

    public MavenMetadataEntry(MavenPackageIdentity packageIdentity) => this.PackageIdentity = packageIdentity ?? throw new ArgumentNullException(nameof (packageIdentity));

    public override IEqualityComparer<string> PathEqualityComparer => (IEqualityComparer<string>) MavenFileNameUtility.FileNameStringComparer;

    public IMavenMetadataEntry CreateEntryWithUpdatedFiles(List<IPackageFile> newPackageFiles)
    {
      MavenMetadataEntry withUpdatedFiles = new MavenMetadataEntry(this.PackageIdentity);
      withUpdatedFiles.CopyFrom(this);
      withUpdatedFiles.PackageFilesInternal = (IReadOnlyCollection<MavenPackageFileNew>) newPackageFiles.Cast<MavenPackageFileNew>().ToImmutableList<MavenPackageFileNew>();
      return (IMavenMetadataEntry) withUpdatedFiles;
    }

    public IMavenMetadataEntryWritable CreateWriteable(ICommitLogEntryHeader commitEntry)
    {
      MavenMetadataEntry writeable = new MavenMetadataEntry(this.PackageIdentity);
      writeable.CopyFrom(this);
      writeable.SetUpdatingCommitInfo(commitEntry);
      return (IMavenMetadataEntryWritable) writeable;
    }

    private void CopyFrom(MavenMetadataEntry source)
    {
      this.CopyFrom((MetadataEntryWithFiles<MavenPackageIdentity, MavenPackageFileNew>) source);
      this.PomBytes = source.PomBytes;
    }

    public override ICachablePackageMetadata GetCachableMetadata(IPackageFileRequest request) => this.SourceChain.IsNullOrEmpty<UpstreamSourceInfo>() ? (ICachablePackageMetadata) null : (ICachablePackageMetadata) new CachablePackageMetadata((IMetadataEntry) this, (IStorageId) new UpstreamStorageId(this.SourceChain.First<UpstreamSourceInfo>()), 0L);

    bool IMavenMetadataEntryWritable.AddPackageFile(MavenPackageFileNew packageFile) => this.AddPackageFile(packageFile);

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
