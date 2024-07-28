// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.ICommitEnumeratingStrategy
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BookmarkTokens;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public interface ICommitEnumeratingStrategy
  {
    Task<CommitEntryBatch> GetNextBatchAsync(
      FeedCore feed,
      PackagingCommitId startAfter,
      PackagingCommitId stopAfter);

    Task<ICommitLogEntry> GetNewestCommitAsync(FeedCore feed);

    Task<ICommitLogEntry> GetCommitAsync(FeedCore feed, CommitLogBookmark bookmark);
  }
}
