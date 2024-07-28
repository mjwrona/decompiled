// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ReadFeed.Pagination.CrossPartitionReadFeedAsyncEnumerator
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Pagination;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Tests.Pagination;
using Microsoft.Azure.Cosmos.Tracing;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ReadFeed.Pagination
{
  internal sealed class CrossPartitionReadFeedAsyncEnumerator : 
    IAsyncEnumerator<TryCatch<CrossFeedRangePage<ReadFeedPage, ReadFeedState>>>,
    IAsyncDisposable
  {
    private readonly CrossPartitionRangePageAsyncEnumerator<ReadFeedPage, ReadFeedState> crossPartitionEnumerator;

    private CrossPartitionReadFeedAsyncEnumerator(
      CrossPartitionRangePageAsyncEnumerator<ReadFeedPage, ReadFeedState> crossPartitionEnumerator)
    {
      this.crossPartitionEnumerator = crossPartitionEnumerator ?? throw new ArgumentNullException(nameof (crossPartitionEnumerator));
    }

    public TryCatch<CrossFeedRangePage<ReadFeedPage, ReadFeedState>> Current { get; set; }

    public ValueTask<bool> MoveNextAsync() => this.MoveNextAsync((ITrace) NoOpTrace.Singleton);

    public async ValueTask<bool> MoveNextAsync(ITrace trace)
    {
      if (trace == null)
        throw new ArgumentNullException(nameof (trace));
      using (ITrace moveNextAsyncTrace = trace.StartChild(nameof (MoveNextAsync), TraceComponent.ReadFeed, TraceLevel.Info))
      {
        if (!await this.crossPartitionEnumerator.MoveNextAsync(moveNextAsyncTrace))
        {
          this.Current = new TryCatch<CrossFeedRangePage<ReadFeedPage, ReadFeedState>>();
          return false;
        }
        TryCatch<CrossFeedRangePage<ReadFeedPage, ReadFeedState>> current = this.crossPartitionEnumerator.Current;
        if (current.Failed)
        {
          this.Current = TryCatch<CrossFeedRangePage<ReadFeedPage, ReadFeedState>>.FromException(current.Exception);
          return true;
        }
        this.Current = TryCatch<CrossFeedRangePage<ReadFeedPage, ReadFeedState>>.FromResult(current.Result);
        return true;
      }
    }

    public ValueTask DisposeAsync() => this.crossPartitionEnumerator.DisposeAsync();

    public void SetCancellationToken(CancellationToken cancellationToken) => this.crossPartitionEnumerator.SetCancellationToken(cancellationToken);

    public static CrossPartitionReadFeedAsyncEnumerator Create(
      IDocumentContainer documentContainer,
      CrossFeedRangeState<ReadFeedState> crossFeedRangeState,
      ReadFeedPaginationOptions readFeedPaginationOptions,
      CancellationToken cancellationToken)
    {
      if (documentContainer == null)
        throw new ArgumentNullException(nameof (documentContainer));
      if (crossFeedRangeState == null)
        throw new ArgumentNullException(nameof (crossFeedRangeState));
      if (readFeedPaginationOptions == null)
        readFeedPaginationOptions = ReadFeedPaginationOptions.Default;
      IComparer<PartitionRangePageAsyncEnumerator<ReadFeedPage, ReadFeedState>> comparer = readFeedPaginationOptions.Direction.GetValueOrDefault(ReadFeedPaginationOptions.PaginationDirection.Forward) != ReadFeedPaginationOptions.PaginationDirection.Forward ? (IComparer<PartitionRangePageAsyncEnumerator<ReadFeedPage, ReadFeedState>>) CrossPartitionReadFeedAsyncEnumerator.PartitionRangePageAsyncEnumeratorComparerReverse.Singleton : (IComparer<PartitionRangePageAsyncEnumerator<ReadFeedPage, ReadFeedState>>) CrossPartitionReadFeedAsyncEnumerator.PartitionRangePageAsyncEnumeratorComparerForward.Singleton;
      return new CrossPartitionReadFeedAsyncEnumerator(new CrossPartitionRangePageAsyncEnumerator<ReadFeedPage, ReadFeedState>((IFeedRangeProvider) documentContainer, CrossPartitionReadFeedAsyncEnumerator.MakeCreateFunction((IReadFeedDataSource) documentContainer, readFeedPaginationOptions, cancellationToken), comparer, new int?(), PrefetchPolicy.PrefetchSinglePage, cancellationToken, crossFeedRangeState));
    }

    private static CreatePartitionRangePageAsyncEnumerator<ReadFeedPage, ReadFeedState> MakeCreateFunction(
      IReadFeedDataSource readFeedDataSource,
      ReadFeedPaginationOptions readFeedPaginationOptions,
      CancellationToken cancellationToken)
    {
      return (CreatePartitionRangePageAsyncEnumerator<ReadFeedPage, ReadFeedState>) (feedRangeState => (PartitionRangePageAsyncEnumerator<ReadFeedPage, ReadFeedState>) new ReadFeedPartitionRangeEnumerator(readFeedDataSource, feedRangeState, readFeedPaginationOptions, cancellationToken));
    }

    private sealed class PartitionRangePageAsyncEnumeratorComparerForward : 
      IComparer<PartitionRangePageAsyncEnumerator<ReadFeedPage, ReadFeedState>>
    {
      public static readonly CrossPartitionReadFeedAsyncEnumerator.PartitionRangePageAsyncEnumeratorComparerForward Singleton = new CrossPartitionReadFeedAsyncEnumerator.PartitionRangePageAsyncEnumeratorComparerForward();

      public int Compare(
        PartitionRangePageAsyncEnumerator<ReadFeedPage, ReadFeedState> partitionRangePageEnumerator1,
        PartitionRangePageAsyncEnumerator<ReadFeedPage, ReadFeedState> partitionRangePageEnumerator2)
      {
        if (partitionRangePageEnumerator1 == partitionRangePageEnumerator2)
          return 0;
        return partitionRangePageEnumerator1.FeedRangeState.FeedRange is FeedRangePartitionKey || partitionRangePageEnumerator2.FeedRangeState.FeedRange is FeedRangePartitionKey ? -1 : string.CompareOrdinal(((FeedRangeEpk) partitionRangePageEnumerator1.FeedRangeState.FeedRange).Range.Min, ((FeedRangeEpk) partitionRangePageEnumerator2.FeedRangeState.FeedRange).Range.Min);
      }
    }

    private sealed class PartitionRangePageAsyncEnumeratorComparerReverse : 
      IComparer<PartitionRangePageAsyncEnumerator<ReadFeedPage, ReadFeedState>>
    {
      public static readonly CrossPartitionReadFeedAsyncEnumerator.PartitionRangePageAsyncEnumeratorComparerReverse Singleton = new CrossPartitionReadFeedAsyncEnumerator.PartitionRangePageAsyncEnumeratorComparerReverse();

      public int Compare(
        PartitionRangePageAsyncEnumerator<ReadFeedPage, ReadFeedState> partitionRangePageEnumerator1,
        PartitionRangePageAsyncEnumerator<ReadFeedPage, ReadFeedState> partitionRangePageEnumerator2)
      {
        return -1 * CrossPartitionReadFeedAsyncEnumerator.PartitionRangePageAsyncEnumeratorComparerForward.Singleton.Compare(partitionRangePageEnumerator1, partitionRangePageEnumerator2);
      }
    }
  }
}
