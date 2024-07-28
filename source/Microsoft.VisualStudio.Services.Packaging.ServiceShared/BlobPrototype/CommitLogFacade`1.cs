// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommitLogFacade`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BookmarkTokens;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class CommitLogFacade<TCommitLogService> : 
    ICommitLog,
    ICommitLogReader<CommitLogEntry>,
    ICommitLogEndpointReader,
    ICommitLogWriter<ICommitLogEntry>
    where TCommitLogService : class, ICommitLogService
  {
    private readonly IVssRequestContext requestContext;
    private readonly IFeedItemStoreContainerProvider containerProvider;

    public CommitLogFacade(
      IVssRequestContext requestContext,
      IFeedItemStoreContainerProvider containerProvider)
    {
      this.requestContext = requestContext;
      this.containerProvider = containerProvider;
    }

    private ICommitLogService CommitLogService => (ICommitLogService) this.requestContext.GetService<TCommitLogService>();

    public async Task<CommitLogBookmark> GetNewestCommitBookmarkAsync(FeedCore feed)
    {
      CommitLogBookmark commitBookmarkAsync = await this.CommitLogService.GetNewestCommitBookmarkAsync(this.requestContext, feed);
      if (commitBookmarkAsync != CommitLogBookmark.Empty)
        this.containerProvider.MarkContainerKnownToExist(feed);
      return commitBookmarkAsync;
    }

    public async Task<CommitLogBookmark> GetOldestCommitBookmarkAsync(FeedCore feed)
    {
      CommitLogBookmark commitBookmarkAsync = await this.CommitLogService.GetOldestCommitBookmarkAsync(this.requestContext, feed);
      if (commitBookmarkAsync != CommitLogBookmark.Empty)
        this.containerProvider.MarkContainerKnownToExist(feed);
      return commitBookmarkAsync;
    }

    public async Task<CommitLogEntry> GetEntryAsync(FeedCore feed, PackagingCommitId commitId)
    {
      CommitLogEntry entryAsync = await this.CommitLogService.GetEntryAsync(this.requestContext, feed, commitId);
      if (entryAsync != null)
        this.containerProvider.MarkContainerKnownToExist(feed);
      return entryAsync;
    }

    public async Task<ICommitLogEntry> AppendEntryAsync(
      FeedCore feed,
      ICommitOperationData operationData)
    {
      await this.containerProvider.EnsureContainerExistsAsync(feed);
      return (ICommitLogEntry) await this.CommitLogService.AppendEntryAsync(this.requestContext, feed, operationData);
    }

    public async Task<IReadOnlyCollection<ICommitLogEntry>> AppendEntriesAsync(
      FeedCore feed,
      IReadOnlyCollection<ICommitOperationData> operations)
    {
      await this.containerProvider.EnsureContainerExistsAsync(feed);
      return (IReadOnlyCollection<ICommitLogEntry>) await this.CommitLogService.AppendEntriesAsync(this.requestContext, feed, operations);
    }

    public async Task MarkCommitLogEntryCorruptAsync(
      FeedCore feed,
      PackagingCommitId commitId,
      string reason = null)
    {
      await this.containerProvider.EnsureContainerExistsAsync(feed);
      await this.CommitLogService.MarkCommitLogEntryCorruptAsync(this.requestContext, feed, commitId, reason);
    }
  }
}
