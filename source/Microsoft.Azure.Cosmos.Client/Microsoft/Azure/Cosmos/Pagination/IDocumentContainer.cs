// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Pagination.IDocumentContainer
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.ChangeFeed.Pagination;
using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.Pagination;
using Microsoft.Azure.Cosmos.ReadFeed.Pagination;
using Microsoft.Azure.Cosmos.Tracing;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Pagination
{
  internal interface IDocumentContainer : 
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
    Task<Record> CreateItemAsync(CosmosObject payload, CancellationToken cancellationToken);

    Task<Record> ReadItemAsync(
      CosmosElement partitionKey,
      string identifier,
      CancellationToken cancellationToken);

    Task SplitAsync(FeedRangeInternal feedRange, CancellationToken cancellationToken);

    Task MergeAsync(
      FeedRangeInternal feedRange1,
      FeedRangeInternal feedRange2,
      CancellationToken cancellationToken);

    Task<string> GetResourceIdentifierAsync(ITrace trace, CancellationToken cancellationToken);
  }
}
