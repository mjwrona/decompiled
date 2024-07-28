// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.Pagination.ChangeFeedPartitionRangePageAsyncEnumerator
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Pagination;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Tracing;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ChangeFeed.Pagination
{
  internal sealed class ChangeFeedPartitionRangePageAsyncEnumerator : 
    PartitionRangePageAsyncEnumerator<ChangeFeedPage, ChangeFeedState>
  {
    private readonly IChangeFeedDataSource changeFeedDataSource;
    private readonly ChangeFeedPaginationOptions changeFeedPaginationOptions;

    public ChangeFeedPartitionRangePageAsyncEnumerator(
      IChangeFeedDataSource changeFeedDataSource,
      Microsoft.Azure.Cosmos.Pagination.FeedRangeState<ChangeFeedState> feedRangeState,
      ChangeFeedPaginationOptions changeFeedPaginationOptions,
      CancellationToken cancellationToken)
      : base(feedRangeState, cancellationToken)
    {
      this.changeFeedDataSource = changeFeedDataSource ?? throw new ArgumentNullException(nameof (changeFeedDataSource));
      this.changeFeedPaginationOptions = changeFeedPaginationOptions ?? throw new ArgumentNullException(nameof (changeFeedPaginationOptions));
    }

    public override ValueTask DisposeAsync() => new ValueTask();

    protected override Task<TryCatch<ChangeFeedPage>> GetNextPageAsync(
      ITrace trace,
      CancellationToken cancellationToken)
    {
      return this.changeFeedDataSource.MonadicChangeFeedAsync(this.FeedRangeState, this.changeFeedPaginationOptions, trace, cancellationToken);
    }
  }
}
