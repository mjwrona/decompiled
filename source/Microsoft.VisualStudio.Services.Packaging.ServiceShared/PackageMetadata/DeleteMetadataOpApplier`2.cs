// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata.DeleteMetadataOpApplier`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata
{
  public class DeleteMetadataOpApplier<TMetadataEntry, TMetadataEntryWriteable> : 
    IgnoreDocumentMetadataEntryOpApplier<TMetadataEntry, MetadataDocument<TMetadataEntry>>
    where TMetadataEntry : class, IMetadataEntry, ICreateWriteable<TMetadataEntryWriteable>
    where TMetadataEntryWriteable : IMetadataEntryWritable, TMetadataEntry
  {
    public override TMetadataEntry Apply(
      ICommitLogEntry commitLogEntry,
      TMetadataEntry currentState)
    {
      if (!(commitLogEntry.CommitOperationData is IDeleteOperationData commitOperationData))
        throw new ArgumentException(Resources.Error_InvalidCommitEntryType());
      if ((object) currentState != null && currentState.DeletedDate.HasValue || (object) currentState != null && currentState.PermanentDeletedDate.HasValue)
        return currentState;
      TMetadataEntryWriteable writeable = currentState.CreateWriteable((ICommitLogEntryHeader) commitLogEntry);
      ref TMetadataEntryWriteable local1 = ref writeable;
      TMetadataEntryWriteable metadataEntryWriteable;
      if ((object) default (TMetadataEntryWriteable) == null)
      {
        metadataEntryWriteable = local1;
        local1 = ref metadataEntryWriteable;
      }
      DateTime? nullable = new DateTime?(commitLogEntry.CreatedDate);
      local1.DeletedDate = nullable;
      ref TMetadataEntryWriteable local2 = ref writeable;
      metadataEntryWriteable = default (TMetadataEntryWriteable);
      if ((object) metadataEntryWriteable == null)
      {
        metadataEntryWriteable = local2;
        local2 = ref metadataEntryWriteable;
      }
      DateTime? permanentDeleteDate = commitOperationData.ScheduledPermanentDeleteDate;
      local2.ScheduledPermanentDeleteDate = permanentDeleteDate;
      return (TMetadataEntry) writeable;
    }
  }
}
