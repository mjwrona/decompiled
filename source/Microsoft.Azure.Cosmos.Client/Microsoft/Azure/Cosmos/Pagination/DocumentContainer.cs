// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Pagination.DocumentContainer
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.ChangeFeed.Pagination;
using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Query.Core;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.Pagination;
using Microsoft.Azure.Cosmos.ReadFeed.Pagination;
using Microsoft.Azure.Cosmos.Tracing;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Pagination
{
  internal sealed class DocumentContainer : 
    IDocumentContainer,
    IMonadicDocumentContainer,
    IMonadicFeedRangeProvider,
    IMonadicQueryDataSource,
    IMonadicReadFeedDataSource,
    IMonadicChangeFeedDataSource,
    IFeedRangeProvider,
    IQueryDataSource,
    IReadFeedDataSource,
    IChangeFeedDataSource
  {
    private readonly IMonadicDocumentContainer monadicDocumentContainer;

    public DocumentContainer(IMonadicDocumentContainer monadicDocumentContainer) => this.monadicDocumentContainer = monadicDocumentContainer ?? throw new ArgumentNullException(nameof (monadicDocumentContainer));

    public Task<TryCatch<List<FeedRangeEpk>>> MonadicGetChildRangeAsync(
      FeedRangeInternal feedRange,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      return this.monadicDocumentContainer.MonadicGetChildRangeAsync(feedRange, trace, cancellationToken);
    }

    public Task<List<FeedRangeEpk>> GetChildRangeAsync(
      FeedRangeInternal feedRange,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      return TryCatch<List<FeedRangeInternal>>.UnsafeGetResultAsync<List<FeedRangeEpk>>(this.MonadicGetChildRangeAsync(feedRange, trace, cancellationToken), cancellationToken);
    }

    public Task<TryCatch<List<FeedRangeEpk>>> MonadicGetFeedRangesAsync(
      ITrace trace,
      CancellationToken cancellationToken)
    {
      return this.monadicDocumentContainer.MonadicGetFeedRangesAsync(trace, cancellationToken);
    }

    public Task<List<FeedRangeEpk>> GetFeedRangesAsync(
      ITrace trace,
      CancellationToken cancellationToken)
    {
      return TryCatch<List<FeedRangeEpk>>.UnsafeGetResultAsync<List<FeedRangeEpk>>(this.MonadicGetFeedRangesAsync(trace, cancellationToken), cancellationToken);
    }

    public Task RefreshProviderAsync(ITrace trace, CancellationToken cancellationToken) => TryCatch.UnsafeWaitAsync(this.MonadicRefreshProviderAsync(trace, cancellationToken), cancellationToken);

    public Task<TryCatch> MonadicRefreshProviderAsync(
      ITrace trace,
      CancellationToken cancellationToken)
    {
      return this.monadicDocumentContainer.MonadicRefreshProviderAsync(trace, cancellationToken);
    }

    public Task<TryCatch<Record>> MonadicCreateItemAsync(
      CosmosObject payload,
      CancellationToken cancellationToken)
    {
      return this.monadicDocumentContainer.MonadicCreateItemAsync(payload, cancellationToken);
    }

    public Task<Record> CreateItemAsync(CosmosObject payload, CancellationToken cancellationToken) => TryCatch<Record>.UnsafeGetResultAsync<Record>(this.MonadicCreateItemAsync(payload, cancellationToken), cancellationToken);

    public Task<TryCatch<Record>> MonadicReadItemAsync(
      CosmosElement partitionKey,
      string identifer,
      CancellationToken cancellationToken)
    {
      return this.monadicDocumentContainer.MonadicReadItemAsync(partitionKey, identifer, cancellationToken);
    }

    public Task<Record> ReadItemAsync(
      CosmosElement partitionKey,
      string identifier,
      CancellationToken cancellationToken)
    {
      return TryCatch<Record>.UnsafeGetResultAsync<Record>(this.MonadicReadItemAsync(partitionKey, identifier, cancellationToken), cancellationToken);
    }

    public Task<TryCatch<ReadFeedPage>> MonadicReadFeedAsync(
      FeedRangeState<ReadFeedState> feedRangeState,
      ReadFeedPaginationOptions readFeedPaginationOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      return this.monadicDocumentContainer.MonadicReadFeedAsync(feedRangeState, readFeedPaginationOptions, trace, cancellationToken);
    }

    public Task<ReadFeedPage> ReadFeedAsync(
      FeedRangeState<ReadFeedState> feedRangeState,
      ReadFeedPaginationOptions readFeedPaginationOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      return TryCatch<ReadFeedPage>.UnsafeGetResultAsync<ReadFeedPage>(this.MonadicReadFeedAsync(feedRangeState, readFeedPaginationOptions, trace, cancellationToken), cancellationToken);
    }

    public Task<TryCatch<QueryPage>> MonadicQueryAsync(
      SqlQuerySpec sqlQuerySpec,
      FeedRangeState<QueryState> feedRangeState,
      QueryPaginationOptions queryPaginationOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      return this.monadicDocumentContainer.MonadicQueryAsync(sqlQuerySpec, feedRangeState, queryPaginationOptions, trace, cancellationToken);
    }

    public Task<QueryPage> QueryAsync(
      SqlQuerySpec sqlQuerySpec,
      FeedRangeState<QueryState> feedRangeState,
      QueryPaginationOptions queryPaginationOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      return TryCatch<QueryPage>.UnsafeGetResultAsync<QueryPage>(this.MonadicQueryAsync(sqlQuerySpec, feedRangeState, queryPaginationOptions, trace, cancellationToken), cancellationToken);
    }

    public Task<TryCatch> MonadicSplitAsync(
      FeedRangeInternal feedRange,
      CancellationToken cancellationToken)
    {
      return this.monadicDocumentContainer.MonadicSplitAsync(feedRange, cancellationToken);
    }

    public Task SplitAsync(FeedRangeInternal feedRange, CancellationToken cancellationToken) => TryCatch.UnsafeWaitAsync(this.MonadicSplitAsync(feedRange, cancellationToken), cancellationToken);

    public Task<TryCatch> MonadicMergeAsync(
      FeedRangeInternal feedRange1,
      FeedRangeInternal feedRange2,
      CancellationToken cancellationToken)
    {
      return this.monadicDocumentContainer.MonadicMergeAsync(feedRange1, feedRange2, cancellationToken);
    }

    public Task MergeAsync(
      FeedRangeInternal feedRange1,
      FeedRangeInternal feedRange2,
      CancellationToken cancellationToken)
    {
      return TryCatch.UnsafeWaitAsync(this.MonadicMergeAsync(feedRange1, feedRange2, cancellationToken), cancellationToken);
    }

    public Task<ChangeFeedPage> ChangeFeedAsync(
      FeedRangeState<ChangeFeedState> feedRangeState,
      ChangeFeedPaginationOptions changeFeedPaginationOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      return TryCatch<ChangeFeedPage>.UnsafeGetResultAsync<ChangeFeedPage>(this.MonadicChangeFeedAsync(feedRangeState, changeFeedPaginationOptions, trace, cancellationToken), cancellationToken);
    }

    public Task<TryCatch<ChangeFeedPage>> MonadicChangeFeedAsync(
      FeedRangeState<ChangeFeedState> state,
      ChangeFeedPaginationOptions changeFeedPaginationOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      return this.monadicDocumentContainer.MonadicChangeFeedAsync(state, changeFeedPaginationOptions, trace, cancellationToken);
    }

    public Task<string> GetResourceIdentifierAsync(
      ITrace trace,
      CancellationToken cancellationToken)
    {
      return TryCatch<string>.UnsafeGetResultAsync<string>(this.MonadicGetResourceIdentifierAsync(trace, cancellationToken), cancellationToken);
    }

    public Task<TryCatch<string>> MonadicGetResourceIdentifierAsync(
      ITrace trace,
      CancellationToken cancellationToken)
    {
      return this.monadicDocumentContainer.MonadicGetResourceIdentifierAsync(trace, cancellationToken);
    }
  }
}
