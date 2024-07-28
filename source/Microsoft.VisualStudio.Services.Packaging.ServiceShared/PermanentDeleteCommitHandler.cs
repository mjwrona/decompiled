// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PermanentDeleteCommitHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class PermanentDeleteCommitHandler : FeedIndexCommitHandler
  {
    public PermanentDeleteCommitHandler(FeedIndexCommitHandler successor = null)
      : base(successor)
    {
    }

    public override Task ApplyCommitAsync(
      IFeedIndexAggregationContext context,
      FeedCore feed,
      ICommitLogEntry commitLogEntry)
    {
      return commitLogEntry.CommitOperationData is IPermanentDeleteOperationData commitOperationData ? this.ApplyPermanentDeleteOperation(context, feed, commitOperationData) : base.ApplyCommitAsync(context, feed, commitLogEntry);
    }

    private async Task ApplyPermanentDeleteOperation(
      IFeedIndexAggregationContext context,
      FeedCore feed,
      IPermanentDeleteOperationData operationData)
    {
      PermanentDeleteCommitHandler sendInTheThisObject = this;
      using (ITracerBlock tracer = context.TracerService.Enter((object) sendInTheThisObject, nameof (ApplyPermanentDeleteOperation)))
      {
        string packageName = operationData.Identity.Name.NormalizedName;
        string packageVersion = operationData.Identity.Version.NormalizedVersion;
        try
        {
          Guid packageId = await FeedIndexCommitHandler.GetFeedPackageId(context, feed, packageName);
          await context.FeedIndexClient.PermanentlyDeletePackageVersionAsync(feed, packageId, await FeedIndexCommitHandler.GetFeedPackageVersion(context, feed, packageId, packageVersion));
          packageId = new Guid();
        }
        catch (PackageNotFoundException ex)
        {
          tracer.TraceInfo(string.Format("Package not found in recycle bin. Feed Id: {0}, Package Name: {1}", (object) feed.Id, (object) packageName));
        }
        catch (PackageVersionNotFoundException ex)
        {
          tracer.TraceInfo(string.Format("Package version not found in recycle bin. Feed Id: {0}, Package Name: {1}, Package Version: {2}", (object) feed.Id, (object) packageName, (object) packageVersion));
        }
        packageName = (string) null;
        packageVersion = (string) null;
      }
    }
  }
}
