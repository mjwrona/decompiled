// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata.UpdateMetadataPackageSizeOpApplier`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata
{
  public class UpdateMetadataPackageSizeOpApplier<TMetadataEntry, TMetadataEntryWriteable> : 
    IgnoreDocumentMetadataEntryOpApplier<TMetadataEntry, MetadataDocument<TMetadataEntry>>
    where TMetadataEntry : class, IMetadataEntry, ICreateWriteable<TMetadataEntryWriteable>
    where TMetadataEntryWriteable : IMetadataEntryWritable, TMetadataEntry
  {
    public override TMetadataEntry Apply(
      ICommitLogEntry commitLogEntry,
      TMetadataEntry currentState)
    {
      if (!(commitLogEntry.CommitOperationData is IUpdatePackageSizeOperationData commitOperationData))
        throw new ArgumentException(Resources.Error_InvalidCommitEntryType());
      // ISSUE: variable of a boxed type
      __Boxed<TMetadataEntry> local1 = (object) currentState;
      if ((local1 != null ? (local1.PackageSize != 0L ? 1 : 0) : 1) != 0)
        return currentState;
      TMetadataEntryWriteable writeable = currentState.CreateWriteable((ICommitLogEntryHeader) commitLogEntry);
      ref TMetadataEntryWriteable local2 = ref writeable;
      if ((object) default (TMetadataEntryWriteable) == null)
      {
        TMetadataEntryWriteable metadataEntryWriteable = local2;
        local2 = ref metadataEntryWriteable;
      }
      long packageSize = commitOperationData.PackageSize;
      local2.PackageSize = packageSize;
      return (TMetadataEntry) writeable;
    }
  }
}
