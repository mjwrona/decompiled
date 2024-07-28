// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Aggregations.PackageMetadata.OperationAppliersNew.MavenAddMetadataOpApplier
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Maven.Server.Implementations.CommitLog.OperationData;
using Microsoft.VisualStudio.Services.Maven.Server.Metadata;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using System;

namespace Microsoft.VisualStudio.Services.Maven.Server.Aggregations.PackageMetadata.OperationAppliersNew
{
  public class MavenAddMetadataOpApplier : 
    IgnoreDocumentMetadataEntryOpApplier<IMavenMetadataEntry, MetadataDocument<IMavenMetadataEntry>>
  {
    public override IMavenMetadataEntry Apply(
      ICommitLogEntry commitLogEntry,
      IMavenMetadataEntry currentState)
    {
      if (!(commitLogEntry.CommitOperationData is MavenCommitOperationData commitOperationData))
        throw new ArgumentException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_InvalidCommitEntryType());
      if (!this.ShouldUpdate(commitOperationData, (IMetadataEntry) currentState))
        return currentState;
      if (currentState == null)
        return (IMavenMetadataEntry) new MavenMetadataEntry(commitOperationData.Identity, commitLogEntry.CommitId, commitLogEntry.CreatedDate, commitLogEntry.ModifiedDate, commitLogEntry.UserId, commitLogEntry.UserId, commitOperationData.SourceChain, commitOperationData.Files, commitOperationData.PomBytes);
      IMavenMetadataEntryWritable writeable = currentState.CreateWriteable((ICommitLogEntryHeader) commitLogEntry);
      bool flag = false;
      foreach (MavenPackageFileNew file in commitOperationData.Files)
        flag |= writeable.AddPackageFile(file);
      if (UpstreamSourceInfoUtils.GetSourceChainKey(commitOperationData.SourceChain) != UpstreamSourceInfoUtils.GetSourceChainKey(((IMetadataEntryWritable) writeable).SourceChain))
      {
        writeable.SourceChain = commitOperationData.SourceChain;
        flag = true;
      }
      if (commitOperationData.PomBytes != null && writeable.PomBytes == null)
      {
        writeable.PomBytes = commitOperationData.PomBytes;
        flag = true;
      }
      return !flag ? currentState : (IMavenMetadataEntry) writeable;
    }

    private bool ShouldUpdate(
      MavenCommitOperationData addOperationData,
      IMetadataEntry currentState)
    {
      if (currentState == null)
        return true;
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
      return true;
label_6:
      return false;
    }
  }
}
