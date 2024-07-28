// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades.CachablePackageMetadata
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System.Collections.Generic;
using System.Collections.Immutable;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades
{
  public class CachablePackageMetadata : ICachablePackageMetadata
  {
    public IStorageId StorageId { get; }

    public IReadOnlyList<UpstreamSourceInfo>? SourceChain { get; }

    public long SizeInBytes { get; }

    public static ICachablePackageMetadata TryAllUpstreams { get; } = (ICachablePackageMetadata) new CachablePackageMetadata((IEnumerable<UpstreamSourceInfo>) null, (IStorageId) new TryAllUpstreamsStorageId(), 0L);

    private CachablePackageMetadata(
      IEnumerable<UpstreamSourceInfo>? sourceChain,
      IStorageId storageId,
      long packageSize)
    {
      this.StorageId = storageId;
      this.SourceChain = sourceChain != null ? (IReadOnlyList<UpstreamSourceInfo>) sourceChain.ToImmutableList<UpstreamSourceInfo>() : (IReadOnlyList<UpstreamSourceInfo>) null;
      this.SizeInBytes = packageSize;
    }

    public CachablePackageMetadata(IMetadataEntry metadataEntry)
      : this(metadataEntry.SourceChain, metadataEntry.PackageStorageId, metadataEntry.PackageSize)
    {
    }

    public CachablePackageMetadata(IMetadataEntry metadataEntry, IPackageFile packageFile)
      : this(metadataEntry.SourceChain, packageFile.StorageId, packageFile.SizeInBytes)
    {
    }

    public CachablePackageMetadata(IMetadataEntry metadataEntry, InnerFileReference innerFile)
      : this(metadataEntry.SourceChain, innerFile.StorageId, innerFile.SizeInBytes)
    {
    }

    public CachablePackageMetadata(
      IMetadataEntry metadataEntry,
      IStorageId storageId,
      long packageSize)
      : this(metadataEntry.SourceChain, storageId, packageSize)
    {
    }
  }
}
