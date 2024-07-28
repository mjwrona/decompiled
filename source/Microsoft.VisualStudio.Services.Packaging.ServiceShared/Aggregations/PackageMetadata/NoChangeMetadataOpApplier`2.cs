// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.PackageMetadata.NoChangeMetadataOpApplier`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.PackageMetadata
{
  public class NoChangeMetadataOpApplier<TMetadataEntry, TMetadataDocument> : 
    IMetadataEntryOpApplier<TMetadataEntry, TMetadataDocument>
    where TMetadataEntry : IMetadataEntry
  {
    public TMetadataEntry Apply(
      ICommitLogEntry commitLogEntry,
      TMetadataEntry currentState,
      TMetadataDocument document)
    {
      return currentState;
    }
  }
}
