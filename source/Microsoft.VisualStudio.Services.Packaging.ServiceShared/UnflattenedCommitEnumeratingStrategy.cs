// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UnflattenedCommitEnumeratingStrategy
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BookmarkTokens;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class UnflattenedCommitEnumeratingStrategy : IUnflattenedCommitEnumeratingStrategy
  {
    private readonly ICommitLogReader<CommitLogEntry> commitLogReader;

    public UnflattenedCommitEnumeratingStrategy(ICommitLogReader<CommitLogEntry> commitLogReader) => this.commitLogReader = commitLogReader;

    public async Task<IConcurrentIterator<ICommitLogEntry>> EnumerateCommitsAsync(
      FeedCore feed,
      PackagingCommitId startAfter,
      PackagingCommitId stopAfter)
    {
      PackagingCommitId nextCommitToProcess;
      if (startAfter == PackagingCommitId.Empty)
      {
        nextCommitToProcess = (await this.commitLogReader.GetOldestCommitBookmarkAsync(feed)).CommitId;
        if (nextCommitToProcess == PackagingCommitId.Empty)
        {
          if (stopAfter != PackagingCommitId.Empty)
            throw new ArgumentException("Commit log is known not to be empty but oldest commit pointer is empty.");
          return (IConcurrentIterator<ICommitLogEntry>) new ConcurrentIterator<ICommitLogEntry>(Enumerable.Empty<ICommitLogEntry>());
        }
      }
      else
        nextCommitToProcess = (await this.commitLogReader.GetEntryAsync(feed, startAfter)).NextCommitId;
      return (IConcurrentIterator<ICommitLogEntry>) new UnflattenedCommitEnumeratingStrategy.CommitLogConcurrentIterator(this.commitLogReader, feed, nextCommitToProcess, stopAfter);
    }

    public async Task<ICommitLogEntry> GetNewestCommitAsync(FeedCore feed)
    {
      CommitLogBookmark commitBookmarkAsync = await this.commitLogReader.GetNewestCommitBookmarkAsync(feed);
      return await this.GetCommitAsync(feed, commitBookmarkAsync);
    }

    public async Task<ICommitLogEntry> GetCommitAsync(FeedCore feed, CommitLogBookmark bookmark) => (ICommitLogEntry) await this.commitLogReader.GetEntryAsync(feed, bookmark.CommitId);

    private class CommitLogConcurrentIterator : IConcurrentIterator<ICommitLogEntry>, IDisposable
    {
      private readonly ICommitLogReader<CommitLogEntry> commitLogReader;
      private readonly FeedCore feed;
      private readonly PackagingCommitId stopAfter;
      private PackagingCommitId nextCommitToProcess;

      public CommitLogConcurrentIterator(
        ICommitLogReader<CommitLogEntry> commitLogReader,
        FeedCore feed,
        PackagingCommitId nextCommitToProcess,
        PackagingCommitId stopAfter)
      {
        this.commitLogReader = commitLogReader;
        this.feed = feed;
        this.nextCommitToProcess = nextCommitToProcess;
        this.stopAfter = stopAfter;
      }

      public ICommitLogEntry Current { get; private set; }

      public bool EnumerationStarted { get; private set; }

      public async Task<bool> MoveNextAsync(CancellationToken token)
      {
        this.EnumerationStarted = true;
        while (!(this.nextCommitToProcess == PackagingCommitId.Empty))
        {
          CommitLogEntry entryAsync = await this.commitLogReader.GetEntryAsync(this.feed, this.nextCommitToProcess);
          this.Current = (ICommitLogEntry) entryAsync;
          this.nextCommitToProcess = entryAsync.CommitId == this.stopAfter ? PackagingCommitId.Empty : entryAsync.NextCommitId;
          ICommitLogEntry current = this.Current;
          if ((current != null ? (current.CorruptEntry ? 1 : 0) : 0) == 0)
            return true;
        }
        this.Current = (ICommitLogEntry) null;
        return false;
      }

      public void Dispose()
      {
      }
    }
  }
}
