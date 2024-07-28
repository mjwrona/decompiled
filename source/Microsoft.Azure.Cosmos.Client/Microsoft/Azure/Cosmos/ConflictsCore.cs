// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ConflictsCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal abstract class ConflictsCore : Conflicts
  {
    private readonly ContainerInternal container;

    public ConflictsCore(CosmosClientContext clientContext, ContainerInternal container)
    {
      if (clientContext == null)
        throw new ArgumentNullException(nameof (clientContext));
      this.container = container != null ? container : throw new ArgumentNullException(nameof (container));
      this.ClientContext = clientContext;
    }

    protected CosmosClientContext ClientContext { get; }

    public Task<ResponseMessage> DeleteAsync(
      ConflictProperties conflict,
      PartitionKey partitionKey,
      ITrace trace,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (conflict == null)
        throw new ArgumentNullException(nameof (conflict));
      return this.ClientContext.ProcessResourceOperationStreamAsync(this.ClientContext.CreateLink(this.container.LinkUri, "conflicts", conflict.Id), ResourceType.Conflict, OperationType.Delete, (RequestOptions) null, this.container, (FeedRange) new FeedRangePartitionKey(partitionKey), (Stream) null, (Action<RequestMessage>) null, trace, cancellationToken);
    }

    public override FeedIterator GetConflictQueryStreamIterator(
      string queryText,
      string continuationToken,
      QueryRequestOptions requestOptions)
    {
      QueryDefinition queryDefinition = (QueryDefinition) null;
      if (queryText != null)
        queryDefinition = new QueryDefinition(queryText);
      return this.GetConflictQueryStreamIterator(queryDefinition, continuationToken, requestOptions);
    }

    public override FeedIterator<T> GetConflictQueryIterator<T>(
      string queryText,
      string continuationToken,
      QueryRequestOptions requestOptions)
    {
      QueryDefinition queryDefinition = (QueryDefinition) null;
      if (queryText != null)
        queryDefinition = new QueryDefinition(queryText);
      return this.GetConflictQueryIterator<T>(queryDefinition, continuationToken, requestOptions);
    }

    public override FeedIterator GetConflictQueryStreamIterator(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      return (FeedIterator) this.container.GetReadFeedIterator(queryDefinition, requestOptions, this.container.LinkUri, ResourceType.Conflict, continuationToken, (int?) requestOptions?.MaxItemCount ?? int.MaxValue);
    }

    public override FeedIterator<T> GetConflictQueryIterator<T>(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      if (!(this.GetConflictQueryStreamIterator(queryDefinition, continuationToken, requestOptions) is FeedIteratorInternal queryStreamIterator))
        throw new InvalidOperationException("Expected a FeedIteratorInternal.");
      return (FeedIterator<T>) new FeedIteratorCore<T>(queryStreamIterator, (Func<ResponseMessage, FeedResponse<T>>) (response => this.ClientContext.ResponseFactory.CreateQueryFeedResponse<T>(response, ResourceType.Conflict)));
    }

    public async Task<ItemResponse<T>> ReadCurrentAsync<T>(
      ConflictProperties cosmosConflict,
      PartitionKey partitionKey,
      ITrace trace,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (cosmosConflict == null)
        throw new ArgumentNullException(nameof (cosmosConflict));
      string databaseResourceId = await ((DatabaseInternal) this.container.Database).GetRIDAsync(cancellationToken);
      string cachedRidAsync = await this.container.GetCachedRIDAsync(false, trace, cancellationToken);
      ItemResponse<T> itemResponse = this.ClientContext.ResponseFactory.CreateItemResponse<T>(await this.ClientContext.ProcessResourceOperationStreamAsync(this.ClientContext.CreateLink(this.ClientContext.CreateLink(this.ClientContext.CreateLink(string.Empty, "dbs", databaseResourceId), "colls", cachedRidAsync), "docs", cosmosConflict.SourceResourceId), ResourceType.Document, OperationType.Read, (RequestOptions) null, this.container, (FeedRange) new FeedRangePartitionKey(partitionKey), (Stream) null, (Action<RequestMessage>) null, trace, cancellationToken));
      databaseResourceId = (string) null;
      return itemResponse;
    }

    public override T ReadConflictContent<T>(ConflictProperties cosmosConflict)
    {
      if (cosmosConflict == null)
        throw new ArgumentNullException(nameof (cosmosConflict));
      if (string.IsNullOrEmpty(cosmosConflict.Content))
        return default (T);
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (StreamWriter streamWriter = new StreamWriter((Stream) memoryStream))
        {
          streamWriter.Write(cosmosConflict.Content);
          streamWriter.Flush();
          memoryStream.Position = 0L;
          return this.ClientContext.SerializerCore.FromStream<T>((Stream) memoryStream);
        }
      }
    }
  }
}
