// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NuGetListMetadataOpApplier
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class NuGetListMetadataOpApplier : 
    IgnoreDocumentMetadataEntryOpApplier<INuGetMetadataEntry, MetadataDocument<INuGetMetadataEntry>>
  {
    private readonly bool list;

    public NuGetListMetadataOpApplier(bool list) => this.list = list;

    public override INuGetMetadataEntry Apply(
      ICommitLogEntry commitLogEntry,
      INuGetMetadataEntry currentState)
    {
      if (currentState != null && currentState.DeletedDate.HasValue || currentState != null && currentState.PermanentDeletedDate.HasValue)
        return currentState;
      INuGetMetadataEntryWriteable writeable = currentState.CreateWriteable((ICommitLogEntryHeader) commitLogEntry);
      writeable.Listed = this.list;
      return (INuGetMetadataEntry) writeable;
    }
  }
}
