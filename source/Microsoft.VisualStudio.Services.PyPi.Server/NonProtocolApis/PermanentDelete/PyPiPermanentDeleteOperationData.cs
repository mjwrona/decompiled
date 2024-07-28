// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.NonProtocolApis.PermanentDelete.PyPiPermanentDeleteOperationData
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobStore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.PyPi.Server.NonProtocolApis.PermanentDelete
{
  public class PyPiPermanentDeleteOperationData : 
    IPyPiPermanentDeleteOperationData,
    IPermanentDeleteOperationData,
    IContentDeleteOperationData,
    IPackageVersionOperationData,
    IPackageOperationData,
    ICommitOperationData
  {
    public PyPiPermanentDeleteOperationData(
      PyPiPackageIdentity packageIdentity,
      IEnumerable<BlobReferenceIdentifier> extraAssetsBlobReferences)
    {
      ArgumentUtility.CheckForNull<PyPiPackageIdentity>(packageIdentity, nameof (packageIdentity));
      this.Identity = packageIdentity;
      this.BlobReferencesToDelete = extraAssetsBlobReferences;
    }

    public IPackageName PackageName => (IPackageName) this.Identity.Name;

    IPackageIdentity IPackageVersionOperationData.Identity => (IPackageIdentity) this.Identity;

    public PyPiPackageIdentity Identity { get; }

    public RingOrder RingOrder => RingOrder.OuterToInner;

    public FeedPermissionConstants PermissionDemand => FeedPermissionConstants.DeletePackage;

    public IStorageId StorageId => (IStorageId) null;

    public IEnumerable<BlobReferenceIdentifier> BlobReferencesToDelete { get; }
  }
}
