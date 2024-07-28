// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.CommitLog.NpmPermanentDeleteOperationData
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobStore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Npm.Server.CommitLog
{
  public class NpmPermanentDeleteOperationData : 
    INpmPermanentDeleteOperationData,
    IPermanentDeleteOperationData,
    IContentDeleteOperationData,
    IPackageVersionOperationData,
    IPackageOperationData,
    ICommitOperationData
  {
    public NpmPermanentDeleteOperationData(
      NpmPackageIdentity packageIdentity,
      IStorageId storageId,
      IEnumerable<BlobReferenceIdentifier> extraAssetsBlobReferences)
    {
      this.Identity = packageIdentity;
      this.StorageId = storageId;
      this.BlobReferencesToDelete = extraAssetsBlobReferences;
    }

    public IPackageName PackageName => (IPackageName) this.Identity.Name;

    IPackageIdentity IPackageVersionOperationData.Identity => (IPackageIdentity) this.Identity;

    public NpmPackageIdentity Identity { get; }

    public RingOrder RingOrder => RingOrder.OuterToInner;

    public FeedPermissionConstants PermissionDemand => FeedPermissionConstants.DeletePackage;

    public IStorageId StorageId { get; }

    public IEnumerable<BlobReferenceIdentifier> BlobReferencesToDelete { get; }
  }
}
