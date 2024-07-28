// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.Pagination.CrossPartitionChangeFeedAsyncEnumerator
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Pagination;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Tracing;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ChangeFeed.Pagination
{
  internal sealed class CrossPartitionChangeFeedAsyncEnumerator : 
    IAsyncEnumerator<TryCatch<CrossFeedRangePage<ChangeFeedPage, ChangeFeedState>>>,
    IAsyncDisposable
  {
    private readonly CrossPartitionRangePageAsyncEnumerator<ChangeFeedPage, ChangeFeedState> crossPartitionEnumerator;
    private CancellationToken cancellationToken;
    private TryCatch<CrossFeedRangePage<ChangeFeedPage, ChangeFeedState>>? bufferedException;

    private CrossPartitionChangeFeedAsyncEnumerator(
      CrossPartitionRangePageAsyncEnumerator<ChangeFeedPage, ChangeFeedState> crossPartitionEnumerator,
      CancellationToken cancellationToken)
    {
      this.crossPartitionEnumerator = crossPartitionEnumerator ?? throw new ArgumentNullException(nameof (crossPartitionEnumerator));
      this.cancellationToken = cancellationToken;
    }

    public TryCatch<CrossFeedRangePage<ChangeFeedPage, ChangeFeedState>> Current { get; private set; }

    public ValueTask DisposeAsync() => this.crossPartitionEnumerator.DisposeAsync();

    public ValueTask<bool> MoveNextAsync() => this.MoveNextAsync((ITrace) NoOpTrace.Singleton);

    public async ValueTask<bool> MoveNextAsync(ITrace trace)
    {
      this.cancellationToken.ThrowIfCancellationRequested();
      if (trace == null)
        throw new ArgumentNullException(nameof (trace));
      using (ITrace changeFeedMoveNextTrace = trace.StartChild("ChangeFeed MoveNextAsync", TraceComponent.ChangeFeed, TraceLevel.Info))
      {
        if (this.bufferedException.HasValue)
        {
          this.Current = this.bufferedException.Value;
          this.bufferedException = new TryCatch<CrossFeedRangePage<ChangeFeedPage, ChangeFeedState>>?();
          return true;
        }
        TryCatch<CrossFeedRangePage<ChangeFeedPage, ChangeFeedState>> tryCatch1 = await this.crossPartitionEnumerator.MoveNextAsync(changeFeedMoveNextTrace) ? this.crossPartitionEnumerator.Current : throw new InvalidOperationException("ChangeFeed should always have a next page.");
        if (tryCatch1.Failed)
        {
          this.Current = TryCatch<CrossFeedRangePage<ChangeFeedPage, ChangeFeedState>>.FromException(tryCatch1.Exception);
          return true;
        }
        CrossFeedRangePage<ChangeFeedPage, ChangeFeedState> crossFeedRangePage = tryCatch1.Result;
        ChangeFeedPage backendPage = crossFeedRangePage.Page;
        if (backendPage is ChangeFeedNotModifiedPage)
        {
          FeedRangeInternal originalRange = this.crossPartitionEnumerator.CurrentRange;
          if (!CrossPartitionChangeFeedAsyncEnumerator.IsNextRangeEqualToOriginal(this.crossPartitionEnumerator, originalRange))
          {
            using (ITrace drainNotModifedPages = changeFeedMoveNextTrace.StartChild("Drain NotModified Pages", TraceComponent.ChangeFeed, TraceLevel.Info))
            {
              double totalRequestCharge = backendPage.RequestCharge;
              do
              {
                TryCatch<CrossFeedRangePage<ChangeFeedPage, ChangeFeedState>> tryCatch2 = await this.crossPartitionEnumerator.MoveNextAsync(drainNotModifedPages) ? this.crossPartitionEnumerator.Current : throw new InvalidOperationException("ChangeFeed should always have a next page.");
                if (tryCatch2.Failed)
                {
                  this.bufferedException = new TryCatch<CrossFeedRangePage<ChangeFeedPage, ChangeFeedState>>?(TryCatch<CrossFeedRangePage<ChangeFeedPage, ChangeFeedState>>.FromException(tryCatch2.Exception));
                }
                else
                {
                  crossFeedRangePage = tryCatch2.Result;
                  backendPage = crossFeedRangePage.Page;
                  totalRequestCharge += backendPage.RequestCharge;
                }
              }
              while (!(backendPage is ChangeFeedSuccessPage) && !CrossPartitionChangeFeedAsyncEnumerator.IsNextRangeEqualToOriginal(this.crossPartitionEnumerator, originalRange) && !this.bufferedException.HasValue);
              backendPage = !(backendPage is ChangeFeedSuccessPage changeFeedSuccessPage) ? (ChangeFeedPage) new ChangeFeedNotModifiedPage(totalRequestCharge, backendPage.ActivityId, backendPage.AdditionalHeaders, backendPage.State) : (ChangeFeedPage) new ChangeFeedSuccessPage(changeFeedSuccessPage.Content, totalRequestCharge, changeFeedSuccessPage.ActivityId, changeFeedSuccessPage.AdditionalHeaders, changeFeedSuccessPage.State);
            }
          }
          originalRange = (FeedRangeInternal) null;
        }
        crossFeedRangePage = new CrossFeedRangePage<ChangeFeedPage, ChangeFeedState>(backendPage, crossFeedRangePage.State);
        this.Current = TryCatch<CrossFeedRangePage<ChangeFeedPage, ChangeFeedState>>.FromResult(crossFeedRangePage);
        return true;
      }
    }

    public void SetCancellationToken(CancellationToken cancellationToken)
    {
      this.cancellationToken = cancellationToken;
      this.crossPartitionEnumerator.SetCancellationToken(cancellationToken);
    }

    public static CrossPartitionChangeFeedAsyncEnumerator Create(
      IDocumentContainer documentContainer,
      CrossFeedRangeState<ChangeFeedState> state,
      ChangeFeedPaginationOptions changeFeedPaginationOptions,
      CancellationToken cancellationToken)
    {
      if (changeFeedPaginationOptions == null)
        changeFeedPaginationOptions = ChangeFeedPaginationOptions.Default;
      if (documentContainer == null)
        throw new ArgumentNullException(nameof (documentContainer));
      return new CrossPartitionChangeFeedAsyncEnumerator(new CrossPartitionRangePageAsyncEnumerator<ChangeFeedPage, ChangeFeedState>((IFeedRangeProvider) documentContainer, CrossPartitionChangeFeedAsyncEnumerator.MakeCreateFunction((IChangeFeedDataSource) documentContainer, changeFeedPaginationOptions, cancellationToken), (IComparer<PartitionRangePageAsyncEnumerator<ChangeFeedPage, ChangeFeedState>>) null, new int?(), PrefetchPolicy.PrefetchSinglePage, cancellationToken, state), cancellationToken);
    }

    private static bool IsNextRangeEqualToOriginal(
      CrossPartitionRangePageAsyncEnumerator<ChangeFeedPage, ChangeFeedState> crossPartitionEnumerator,
      FeedRangeInternal originalRange)
    {
      FeedRangeState<ChangeFeedState> nextState;
      return crossPartitionEnumerator.TryPeekNext(out nextState) && originalRange.Equals((object) nextState.FeedRange);
    }

    private static CreatePartitionRangePageAsyncEnumerator<ChangeFeedPage, ChangeFeedState> MakeCreateFunction(
      IChangeFeedDataSource changeFeedDataSource,
      ChangeFeedPaginationOptions changeFeedPaginationOptions,
      CancellationToken cancellationToken)
    {
      return (CreatePartitionRangePageAsyncEnumerator<ChangeFeedPage, ChangeFeedState>) (feedRangeState => (PartitionRangePageAsyncEnumerator<ChangeFeedPage, ChangeFeedState>) new ChangeFeedPartitionRangePageAsyncEnumerator(changeFeedDataSource, feedRangeState, changeFeedPaginationOptions, cancellationToken));
    }
  }
}
