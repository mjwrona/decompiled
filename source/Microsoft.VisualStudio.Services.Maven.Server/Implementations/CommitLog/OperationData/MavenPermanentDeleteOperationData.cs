// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Implementations.CommitLog.OperationData.MavenPermanentDeleteOperationData
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobStore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Maven.Server.Implementations.CommitLog.OperationData
{
  public class MavenPermanentDeleteOperationData : 
    IMavenPermanentDeleteOperationData,
    IPermanentDeleteOperationData,
    IContentDeleteOperationData,
    IPackageVersionOperationData,
    IPackageOperationData,
    ICommitOperationData
  {
    public MavenPermanentDeleteOperationData(
      MavenPackageIdentity packageIdentity,
      IEnumerable<BlobReferenceIdentifier> extraAssetsBlobReferences)
    {
      ArgumentUtility.CheckForNull<MavenPackageIdentity>(packageIdentity, nameof (packageIdentity));
      this.Identity = packageIdentity;
      this.BlobReferencesToDelete = extraAssetsBlobReferences;
    }

    public IPackageName PackageName => (IPackageName) this.Identity.Name;

    IPackageIdentity IPackageVersionOperationData.Identity => (IPackageIdentity) this.Identity;

    public MavenPackageIdentity Identity { get; }

    public RingOrder RingOrder => RingOrder.OuterToInner;

    public FeedPermissionConstants PermissionDemand => FeedPermissionConstants.DeletePackage;

    public IStorageId StorageId => (IStorageId) null;

    public IEnumerable<BlobReferenceIdentifier> BlobReferencesToDelete { get; }
  }
}
