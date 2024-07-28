// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.MultipleFilePackageTypeAddCommitHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class MultipleFilePackageTypeAddCommitHandler : FeedIndexCommitHandler
  {
    public MultipleFilePackageTypeAddCommitHandler(FeedIndexCommitHandler successor = null)
      : base(successor)
    {
    }

    public override Task ApplyCommitAsync(
      IFeedIndexAggregationContext context,
      FeedCore feed,
      ICommitLogEntry commitLogEntry)
    {
      return commitLogEntry.CommitOperationData is IAddOperationData commitOperationData ? MultipleFilePackageTypeAddCommitHandler.ApplyAddOperation(context, feed, commitOperationData, commitLogEntry) : base.ApplyCommitAsync(context, feed, commitLogEntry);
    }

    private static async Task ApplyAddOperation(
      IFeedIndexAggregationContext context,
      FeedCore feed,
      IAddOperationData op,
      ICommitLogEntry commitLogEntry)
    {
      string normalizedName = op.Identity.Name.NormalizedName;
      string normalizedPackageVersion = op.Identity.Version.NormalizedVersion;
      PackageVersion packageVersion = (PackageVersion) null;
      Package package = await context.FeedIndexClient.GetPackageAsync(feed, context.Protocol.ToString(), normalizedName, true, false);
      if (package != null)
        packageVersion = await context.FeedIndexClient.GetPackageVersionAsync(feed, package.Id, normalizedPackageVersion, false);
      try
      {
        if (packageVersion == null)
        {
          await AddCommitHandler.ApplyAddOperation(context, feed, op, commitLogEntry);
          normalizedPackageVersion = (string) null;
          packageVersion = (PackageVersion) null;
          package = (Package) null;
        }
        else
        {
          IEnumerable<PackageFile> files = (IEnumerable<PackageFile>) op.ConvertToIndexEntry(commitLogEntry, feed).PackageVersion.Files.CombineWith<PackageFile>(packageVersion.Files, StringComparison.Ordinal);
          await context.FeedIndexClient.UpdatePackageVersionAsync(feed, package.Id.ToString(), normalizedPackageVersion, files);
          normalizedPackageVersion = (string) null;
          packageVersion = (PackageVersion) null;
          package = (Package) null;
        }
      }
      catch (PackageVersionDeletedException ex)
      {
        normalizedPackageVersion = (string) null;
        packageVersion = (PackageVersion) null;
        package = (Package) null;
      }
    }
  }
}
