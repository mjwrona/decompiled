// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.FeedIndexCommitHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public abstract class FeedIndexCommitHandler
  {
    protected readonly FeedIndexCommitHandler successor;

    public FeedIndexCommitHandler(FeedIndexCommitHandler successor = null) => this.successor = successor;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual Task ApplyCommitAsync(
      IFeedIndexAggregationContext context,
      FeedCore feed,
      ICommitLogEntry commitLogEntry)
    {
      return this.successor?.ApplyCommitAsync(context, feed, commitLogEntry) ?? Task.CompletedTask;
    }

    protected static async Task<Guid> GetFeedPackageId(
      IFeedIndexAggregationContext context,
      FeedCore feed,
      string normalizedName)
    {
      return (await context.FeedIndexClient.GetPackageAsync(feed, context.Protocol.ToString(), normalizedName, true, true)).Id;
    }

    protected static async Task<Guid> GetFeedPackageVersion(
      IFeedIndexAggregationContext context,
      FeedCore feed,
      Guid packageId,
      string packageVersion)
    {
      return (await context.FeedIndexClient.GetPackageVersionAsync(feed, packageId, packageVersion, true)).Id;
    }
  }
}
