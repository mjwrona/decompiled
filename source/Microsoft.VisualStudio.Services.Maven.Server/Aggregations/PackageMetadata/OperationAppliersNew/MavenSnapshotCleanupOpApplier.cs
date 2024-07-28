// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Aggregations.PackageMetadata.OperationAppliersNew.MavenSnapshotCleanupOpApplier
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Maven.Server.Contracts;
using Microsoft.VisualStudio.Services.Maven.Server.Metadata;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Maven.Server.Models.Xml.Helpers;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Maven.Server.Aggregations.PackageMetadata.OperationAppliersNew
{
  public class MavenSnapshotCleanupOpApplier : 
    IgnoreDocumentMetadataEntryOpApplier<IMavenMetadataEntry, MetadataDocument<IMavenMetadataEntry>>
  {
    public override IMavenMetadataEntry Apply(
      ICommitLogEntry commitLogEntry,
      IMavenMetadataEntry currentState)
    {
      if (!(commitLogEntry.CommitOperationData is IMavenSnapshotCleanupOperationData commitOperationData))
        throw new ArgumentException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_InvalidCommitEntryType());
      DateTime? nullable;
      if (currentState != null)
      {
        nullable = currentState.DeletedDate;
        if (nullable.HasValue)
          goto label_6;
      }
      if (currentState != null)
      {
        nullable = currentState.PermanentDeletedDate;
        if (nullable.HasValue)
          goto label_6;
      }
      IMavenMetadataEntryWritable writeable = currentState.CreateWriteable((ICommitLogEntryHeader) commitLogEntry);
      List<IPackageFile> newPackageFiles = new List<IPackageFile>((IEnumerable<IPackageFile>) writeable.PackageFiles.Except<MavenPackageFileNew>(this.ComputeFilesToRemove((IMavenMetadataEntry) writeable, (IEnumerable<MavenSnapshotInstanceId>) commitOperationData.SnapshotInstanceIds)));
      return writeable.CreateEntryWithUpdatedFiles(newPackageFiles);
label_6:
      return currentState;
    }

    private IEnumerable<MavenPackageFileNew> ComputeFilesToRemove(
      IMavenMetadataEntry metadataEntry,
      IEnumerable<MavenSnapshotInstanceId> snapshotInstancesToRemove)
    {
      return new MavenSnapshotMetadataFiles<MavenPackageFileNew>(metadataEntry.PackageIdentity.Name, (IEnumerable<MavenPackageFileNew>) metadataEntry.PackageFiles, (Func<MavenPackageFileNew, string>) (x => x.Path)).FilterFilesByInstances(snapshotInstancesToRemove);
    }
  }
}
