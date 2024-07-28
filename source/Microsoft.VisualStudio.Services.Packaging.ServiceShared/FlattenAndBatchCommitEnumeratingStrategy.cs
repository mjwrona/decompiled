// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.FlattenAndBatchCommitEnumeratingStrategy
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BookmarkTokens;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class FlattenAndBatchCommitEnumeratingStrategy : ICommitEnumeratingStrategy
  {
    private readonly ICommitLogReader<CommitLogEntry> commitLogReader;
    private readonly IFactory<int> batchSizeFactory;

    public FlattenAndBatchCommitEnumeratingStrategy(
      ICommitLogReader<CommitLogEntry> commitLogReader,
      IFactory<int> batchSizeFactory)
    {
      this.commitLogReader = commitLogReader;
      this.batchSizeFactory = batchSizeFactory;
    }

    public async Task<CommitEntryBatch> GetNextBatchAsync(
      FeedCore feed,
      PackagingCommitId startAfter,
      PackagingCommitId stopAfter)
    {
      List<ICommitLogEntry> commitEntriesProcessed = new List<ICommitLogEntry>();
      CommitLogBookmark lastConsultedBookmark = CommitLogBookmark.Empty;
      PackagingCommitId currIdToProcess;
      if (startAfter == PackagingCommitId.Empty)
      {
        currIdToProcess = (await this.commitLogReader.GetOldestCommitBookmarkAsync(feed)).CommitId;
        if (currIdToProcess == PackagingCommitId.Empty)
        {
          if (stopAfter != PackagingCommitId.Empty)
            throw new ArgumentException("Commit log is known not to be empty but oldest commit pointer is empty.");
          return new CommitEntryBatch((IReadOnlyList<ICommitLogEntry>) commitEntriesProcessed, lastConsultedBookmark);
        }
      }
      else
        currIdToProcess = (await this.commitLogReader.GetEntryAsync(feed, startAfter)).NextCommitId;
      int batchSize = this.batchSizeFactory.Get();
      CommitLogEntry entryAsync;
      for (int i = 0; currIdToProcess != PackagingCommitId.Empty && i < batchSize; currIdToProcess = entryAsync.NextCommitId)
      {
        entryAsync = await this.commitLogReader.GetEntryAsync(feed, currIdToProcess);
        if (!entryAsync.CorruptEntry)
        {
          foreach (ICommitLogEntry commitLogEntry in this.GetCommitEntriesFor(entryAsync))
          {
            commitEntriesProcessed.Add(commitLogEntry);
            ++i;
          }
        }
        lastConsultedBookmark = entryAsync.GetCommitLogBookmark();
        if (currIdToProcess == stopAfter)
          break;
      }
      return new CommitEntryBatch((IReadOnlyList<ICommitLogEntry>) commitEntriesProcessed, lastConsultedBookmark);
    }

    private IEnumerable<ICommitLogEntry> GetCommitEntriesFor(CommitLogEntry entry)
    {
      if (entry.CommitOperationData is IBatchCommitOperationData commitOperationData)
      {
        foreach (ICommitOperationData operation in commitOperationData.Operations)
          yield return AggregationAccessorCommonUtils.GetCommitLogEntryForInternalOperation((ICommitLogEntry) entry, operation);
      }
      else
        yield return (ICommitLogEntry) entry;
    }

    public async Task<ICommitLogEntry> GetNewestCommitAsync(FeedCore feed)
    {
      CommitLogBookmark commitBookmarkAsync = await this.commitLogReader.GetNewestCommitBookmarkAsync(feed);
      return await this.GetCommitAsync(feed, commitBookmarkAsync);
    }

    public async Task<ICommitLogEntry> GetCommitAsync(FeedCore feed, CommitLogBookmark bookmark) => (ICommitLogEntry) await this.commitLogReader.GetEntryAsync(feed, bookmark.CommitId);
  }
}
