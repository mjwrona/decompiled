// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.UpdateUpstreamMetadataVersionOpApplier`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class UpdateUpstreamMetadataVersionOpApplier<TPackageIdentity, TMetadataEntry> : 
    IMetadataEntryOpApplier<TMetadataEntry, MetadataDocument<TMetadataEntry>>
    where TPackageIdentity : IPackageIdentity
    where TMetadataEntry : class, IMetadataEntry<TPackageIdentity>
  {
    public TMetadataEntry Apply(
      ICommitLogEntry commitLogEntry,
      TMetadataEntry? currentState,
      MetadataDocument<TMetadataEntry> extraData)
    {
      TMetadataEntry upstreamEntry = ((IUpdateUpstreamMetadataVersionOperationData<TPackageIdentity, TMetadataEntry>) commitLogEntry.CommitOperationData).UpstreamEntry;
      return (object) currentState != null && currentState.IsLocal ? currentState : upstreamEntry;
    }
  }
}
