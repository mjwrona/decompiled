// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations.PermanentDeleteOperationData
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobStore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations
{
  public class PermanentDeleteOperationData : 
    IPermanentDeleteOperationData,
    IContentDeleteOperationData,
    IPackageVersionOperationData,
    IPackageOperationData,
    ICommitOperationData
  {
    public PermanentDeleteOperationData(IPackageIdentity identity, IStorageId storageId)
    {
      this.Identity = identity;
      this.StorageId = storageId;
    }

    public IPackageIdentity Identity { get; }

    public IPackageName PackageName => this.Identity.Name;

    public RingOrder RingOrder => RingOrder.OuterToInner;

    public FeedPermissionConstants PermissionDemand => FeedPermissionConstants.DeletePackage;

    public IStorageId StorageId { get; }

    public IEnumerable<BlobReferenceIdentifier> BlobReferencesToDelete => (IEnumerable<BlobReferenceIdentifier>) null;
  }
}
