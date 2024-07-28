// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.StorageDeleters.StorageDeletionRequest
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobStore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.StorageDeleters
{
  public static class StorageDeletionRequest
  {
    public static IStorageDeletionRequest Create(
      Guid feedId,
      IPackageIdentity packageIdentity,
      IStorageId storageId,
      IEnumerable<BlobReferenceIdentifier> extraBlobAssets)
    {
      Type type;
      if (storageId != null)
        type = typeof (StorageDeletionRequest.GenericImpl<>).MakeGenericType(storageId.GetType());
      else
        type = typeof (StorageDeletionRequest.Impl);
      object[] objArray = new object[4]
      {
        (object) feedId,
        (object) packageIdentity,
        (object) storageId,
        (object) extraBlobAssets
      };
      return (IStorageDeletionRequest) Activator.CreateInstance(type, objArray);
    }

    private class GenericImpl<TStorageId> : 
      IStorageDeletionRequest<TStorageId>,
      IStorageDeletionRequest
      where TStorageId : IStorageId
    {
      public GenericImpl(
        Guid feedId,
        IPackageIdentity packageIdentity,
        TStorageId storageId,
        IEnumerable<BlobReferenceIdentifier> extraBlobAssets)
      {
        this.PackageIdentity = packageIdentity;
        this.StorageId = storageId;
        this.FeedId = feedId;
        this.ExtraAssetBlobReferences = extraBlobAssets;
      }

      public Guid FeedId { get; }

      public IPackageIdentity PackageIdentity { get; }

      public TStorageId StorageId { get; }

      public IEnumerable<BlobReferenceIdentifier> ExtraAssetBlobReferences { get; }

      IStorageId IStorageDeletionRequest.StorageId => (IStorageId) this.StorageId;
    }

    private class Impl : IStorageDeletionRequest
    {
      public Impl(
        Guid feedId,
        IPackageIdentity packageIdentity,
        IStorageId storageId,
        IEnumerable<BlobReferenceIdentifier> extraBlobAssets)
      {
        this.PackageIdentity = packageIdentity;
        this.StorageId = storageId;
        this.FeedId = feedId;
        this.ExtraAssetBlobReferences = extraBlobAssets;
      }

      public Guid FeedId { get; }

      public IPackageIdentity PackageIdentity { get; }

      public IStorageId StorageId { get; }

      public IEnumerable<BlobReferenceIdentifier> ExtraAssetBlobReferences { get; }
    }
  }
}
