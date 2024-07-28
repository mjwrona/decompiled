// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.PermanentDeleteMetadataOpApplier`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class PermanentDeleteMetadataOpApplier<TMetadataEntry, TMetadataEntryWritable> : 
    IgnoreDocumentMetadataEntryOpApplier<TMetadataEntry, MetadataDocument<TMetadataEntry>>
    where TMetadataEntry : class, IMetadataEntry, ICreateWriteable<TMetadataEntryWritable>
    where TMetadataEntryWritable : IMetadataEntryWritable, TMetadataEntry
  {
    public override TMetadataEntry Apply(
      ICommitLogEntry commitLogEntry,
      TMetadataEntry? currentState)
    {
      if (!(commitLogEntry.CommitOperationData is IPermanentDeleteOperationData))
        throw new ArgumentException(Resources.Error_InvalidCommitEntryType());
      if ((object) currentState == null)
        throw new ArgumentException(Resources.Error_NoCurrentStateForNonAddOperation());
      if (currentState.IsPermanentlyDeleted())
        return currentState;
      TMetadataEntryWritable writeable = currentState.CreateWriteable((ICommitLogEntryHeader) commitLogEntry);
      ref TMetadataEntryWritable local = ref writeable;
      if ((object) default (TMetadataEntryWritable) == null)
      {
        TMetadataEntryWritable metadataEntryWritable = local;
        local = ref metadataEntryWritable;
      }
      DateTime? nullable = new DateTime?(commitLogEntry.CreatedDate);
      local.PermanentDeletedDate = nullable;
      return (TMetadataEntry) writeable;
    }
  }
}
