// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations.DeleteOperationData
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations
{
  public class DeleteOperationData : 
    IDeleteOperationData,
    IPackageVersionOperationData,
    IPackageOperationData,
    ICommitOperationData
  {
    public DeleteOperationData(
      IPackageIdentity identity,
      DateTime deletedDate,
      DateTime? scheduledPermanentDeleteDate)
    {
      CommitLogUtils.CheckDateIsUtc(deletedDate, nameof (deletedDate));
      if (scheduledPermanentDeleteDate.HasValue)
        CommitLogUtils.CheckDateIsUtc(scheduledPermanentDeleteDate.Value, nameof (scheduledPermanentDeleteDate));
      this.Identity = identity;
      this.DeletedDate = deletedDate;
      this.ScheduledPermanentDeleteDate = scheduledPermanentDeleteDate;
    }

    public IPackageIdentity Identity { get; }

    public DateTime DeletedDate { get; }

    public DateTime? ScheduledPermanentDeleteDate { get; }

    public RingOrder RingOrder => RingOrder.OuterToInner;

    public FeedPermissionConstants PermissionDemand => FeedPermissionConstants.DeletePackage;

    public IPackageName PackageName => this.Identity.Name;
  }
}
