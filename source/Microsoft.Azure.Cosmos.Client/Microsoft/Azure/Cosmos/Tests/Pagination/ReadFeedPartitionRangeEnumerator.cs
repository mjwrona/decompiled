// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Tests.Pagination.ReadFeedPartitionRangeEnumerator
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Pagination;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.ReadFeed.Pagination;
using Microsoft.Azure.Cosmos.Tracing;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Tests.Pagination
{
  internal sealed class ReadFeedPartitionRangeEnumerator : 
    PartitionRangePageAsyncEnumerator<ReadFeedPage, ReadFeedState>
  {
    private readonly IReadFeedDataSource readFeedDataSource;
    private readonly ReadFeedPaginationOptions readFeedPaginationOptions;

    public ReadFeedPartitionRangeEnumerator(
      IReadFeedDataSource readFeedDataSource,
      Microsoft.Azure.Cosmos.Pagination.FeedRangeState<ReadFeedState> feedRangeState,
      ReadFeedPaginationOptions readFeedPaginationOptions,
      CancellationToken cancellationToken)
      : base(feedRangeState, cancellationToken)
    {
      this.readFeedDataSource = readFeedDataSource ?? throw new ArgumentNullException(nameof (readFeedDataSource));
      this.readFeedPaginationOptions = readFeedPaginationOptions;
    }

    public override ValueTask DisposeAsync() => new ValueTask();

    protected override Task<TryCatch<ReadFeedPage>> GetNextPageAsync(
      ITrace trace,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.readFeedDataSource.MonadicReadFeedAsync(this.FeedRangeState, this.readFeedPaginationOptions, trace, cancellationToken);
    }
  }
}
