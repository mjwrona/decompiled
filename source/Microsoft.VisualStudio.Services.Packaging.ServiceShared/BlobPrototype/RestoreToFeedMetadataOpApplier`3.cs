// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.RestoreToFeedMetadataOpApplier`3
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class RestoreToFeedMetadataOpApplier<TMetadataEntry, TMetadataEntryWritable, TRestoreToFeedOperationData> : 
    IgnoreDocumentMetadataEntryOpApplier<TMetadataEntry, MetadataDocument<TMetadataEntry>>
    where TMetadataEntry : class, IMetadataEntry, ICreateWriteable<TMetadataEntryWritable>
    where TMetadataEntryWritable : IMetadataEntryWritable, TMetadataEntry
    where TRestoreToFeedOperationData : IRestoreToFeedOperationData
  {
    private readonly IHandler<(TMetadataEntryWritable, TRestoreToFeedOperationData), TMetadataEntryWritable>? protocolSpecificWork;

    public RestoreToFeedMetadataOpApplier(
      IHandler<(TMetadataEntryWritable, TRestoreToFeedOperationData), TMetadataEntryWritable>? protocolSpecificWork = null)
    {
      this.protocolSpecificWork = protocolSpecificWork;
    }

    public override TMetadataEntry Apply(
      ICommitLogEntry commitLogEntry,
      TMetadataEntry? currentState)
    {
      if (!(commitLogEntry.CommitOperationData is TRestoreToFeedOperationData commitOperationData))
        throw new ArgumentException(Resources.Error_InvalidCommitEntryType());
      if ((object) currentState == null)
        throw new ArgumentException(Resources.Error_NoCurrentStateForNonAddOperation());
      if (currentState.IsPermanentlyDeleted())
        return currentState;
      TMetadataEntryWritable metadataEntryWritable1 = currentState.CreateWriteable((ICommitLogEntryHeader) commitLogEntry);
      ref TMetadataEntryWritable local1 = ref metadataEntryWritable1;
      TMetadataEntryWritable metadataEntryWritable2;
      if ((object) default (TMetadataEntryWritable) == null)
      {
        metadataEntryWritable2 = local1;
        local1 = ref metadataEntryWritable2;
      }
      DateTime? nullable1 = new DateTime?();
      local1.DeletedDate = nullable1;
      ref TMetadataEntryWritable local2 = ref metadataEntryWritable1;
      metadataEntryWritable2 = default (TMetadataEntryWritable);
      if ((object) metadataEntryWritable2 == null)
      {
        metadataEntryWritable2 = local2;
        local2 = ref metadataEntryWritable2;
      }
      DateTime? nullable2 = new DateTime?();
      local2.ScheduledPermanentDeleteDate = nullable2;
      if (this.protocolSpecificWork != null)
        metadataEntryWritable1 = this.protocolSpecificWork.Handle((metadataEntryWritable1, commitOperationData));
      return (TMetadataEntry) metadataEntryWritable1;
    }
  }
}
