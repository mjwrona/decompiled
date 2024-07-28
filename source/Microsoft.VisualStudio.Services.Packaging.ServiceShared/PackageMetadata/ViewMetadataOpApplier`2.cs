// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata.ViewMetadataOpApplier`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata
{
  public class ViewMetadataOpApplier<TMetadataEntry, TMetadataEntryWriteable> : 
    IgnoreDocumentMetadataEntryOpApplier<TMetadataEntry, MetadataDocument<TMetadataEntry>>
    where TMetadataEntry : class, IMetadataEntry, ICreateWriteable<TMetadataEntryWriteable>
    where TMetadataEntryWriteable : IMetadataEntryWritable, TMetadataEntry
  {
    public override TMetadataEntry Apply(
      ICommitLogEntry commitLogEntry,
      TMetadataEntry currentState)
    {
      if (!(commitLogEntry.CommitOperationData is IViewOperationData commitOperationData))
        throw new ArgumentException(Resources.Error_InvalidCommitEntryType());
      if ((object) currentState != null && currentState.DeletedDate.HasValue || (object) currentState != null && currentState.PermanentDeletedDate.HasValue || commitOperationData.MetadataSuboperation != MetadataSuboperation.Add)
        return currentState;
      HashSet<Guid> guidSet = new HashSet<Guid>(currentState.Views ?? Enumerable.Empty<Guid>())
      {
        commitOperationData.ViewId
      };
      TMetadataEntryWriteable writeable = currentState.CreateWriteable((ICommitLogEntryHeader) commitLogEntry);
      writeable.Views = (IEnumerable<Guid>) guidSet;
      return (TMetadataEntry) writeable;
    }
  }
}
